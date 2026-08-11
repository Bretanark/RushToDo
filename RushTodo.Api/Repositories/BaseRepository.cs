using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;
using RushTodo.Api.Entities;
using RushTodo.Api.Helpers;

namespace RushTodo.Api.Repositories;

public interface IBaseRepository<TEntity>
    where TEntity : Entity, new()
{
    Task<TEntity> Add(TEntity entity, string? auditDescription = "");
    AuditEvent Audit(string description, int entityId);
    AuditEvent? Audit(string description, TEntity entity, bool isNew, params AuditItem[] items);
    Task Delete(int id);
    Task<TEntity> Get(int id);
    Task<TEntity> GetForUpdate(int id);
    Task<TResult> Query<TResult>(Func<IQueryable<TEntity>, Task<TResult>> query);
    Task Save();
    Task<TEntity> Save(int? id, string? auditDescription, Action<TEntity> apply);
    Task<TEntity> Save(IModelEntity? model, string? auditDescription, Action<TEntity> apply);
}


public interface IStaticRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : Entity, new()
{
    Task<Dictionary<int, TEntity>> GetById();
    Task<TEntity[]> List();
    void Invalidate();
    Task Refresh(int id);
}


public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : Entity, new()
{
    private string? _keyPropertyName;

    protected BaseRepository(RushTodoDbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected RushTodoDbContext DbContext { get; }
    protected DbSet<TEntity> Set => DbContext.Set<TEntity>();
    protected virtual IQueryable<TEntity> Entities => Set;
    protected string KeyPropertyName => _keyPropertyName ??= DbContext.Model.FindEntityType(typeof(TEntity))!.FindPrimaryKey()!.Properties.Single().Name;

    public virtual async Task<TEntity> Add(TEntity entity, string? auditDescription = "")
    {
        Set.Add(entity);
        await DbContext.SaveChangesAsync();

        if (auditDescription == null) return entity;

        if (auditDescription == "") auditDescription = "Added";
        Audit(auditDescription, entity.Id);
        await DbContext.SaveChangesAsync();

        return entity;
    }

    public virtual async Task Delete(int id)
    {
        var entity = await GetForUpdate(id);

        if (entity is IDeletableEntity deletableEntity)
            deletableEntity.IsDeleted = true;
        else
            Set.Remove(entity);

        await Save();
    }

    public virtual async Task<TEntity> Get(int id)
    {
        var result = await Set.FindAsync(id);
        return result ?? throw new NotFoundException($"{typeof(TEntity).Name} {id} was not found.");
    }

    public async Task<TEntity> GetForUpdate(int id)
    {
        var result = await Query(query => query.FirstOrDefaultAsync(entity => EF.Property<int>(entity, KeyPropertyName) == id));
        return result ?? throw new NotFoundException($"{typeof(TEntity).Name} {id} was not found.");
    }

    public Task<TResult> Query<TResult>(Func<IQueryable<TEntity>, Task<TResult>> query) => query(Entities);

    public virtual async Task Save()
    {
        var changedEntries = DbContext.ChangeTracker.Entries<TEntity>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Deleted or EntityState.Modified)
            .ToArray();

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 2601 or 2627 })
        {
            throw new UniqueConstraintException(exception);
        }

        await AfterSave(changedEntries);
    }

    public async Task<TEntity> Save(int? id, string? auditDescription, Action<TEntity> apply)
    {
        var model = id is null ? null : await Get(id.Value);
        return await Save(model, auditDescription, GetForUpdate, apply);
    }

    public Task<TEntity> Save(IModelEntity? model, string? auditDescription, Action<TEntity> apply)
        => Save(model, auditDescription, GetForUpdate, apply);

    protected async Task<TEntity> Save(IModelEntity? model, string? auditDescription, Func<int, Task<TEntity>> getForUpdate, Action<TEntity> apply)
    {
        var isNew = (model?.Id ?? 0) == 0;
        if (auditDescription == "") auditDescription = isNew ? "Added" : "Updated";

        var entity = isNew ? new() : await getForUpdate(model!.Id);
        if (!isNew && model!.UpdateDateTime != entity.UpdateDateTime) throw new ConcurrencyException();

        apply(entity);
        var auditItems = isNew ? [] : GetAuditItems(entity);

        if (isNew) Set.Add(entity);

        await Save();

        if (auditDescription == null) return entity;

        Audit(auditDescription, entity.Id, auditItems);
        await Save();

        return entity;
    }

    public AuditEvent Audit(string description, int entityId) => Audit(description, entityId, []);

    public AuditEvent? Audit(string description, TEntity entity, bool isNew, params AuditItem[] items)
    {
        var changedValues = isNew ? [] : GetAuditItems(entity);
        var values = changedValues.Concat(items).ToArray();
        if (!isNew && values.Length == 0) return null;

        return Audit(description, entity.Id, values);
    }

    protected virtual Task AfterSave(EntityEntry<TEntity>[] changedEntries) => Task.CompletedTask;

    private AuditItem[] GetAuditItems(TEntity entity)
    {
        DbContext.ChangeTracker.DetectChanges();

        var entry = DbContext.Entry(entity);
        return entry.Properties
            .Where(property => property.IsModified)
            .Where(property => property.Metadata.PropertyInfo?.IsDefined(typeof(DoNotAuditAttribute), inherit: true) != true)
            .Where(property => !Equals(property.OriginalValue, property.CurrentValue))
            .Select(property => new AuditItem(property.Metadata.Name, FormatValue(property.OriginalValue), FormatValue(property.CurrentValue)))
            .ToArray();
    }

    private AuditEvent Audit(string description, int entityId, AuditItem[] items)
    {
        var auditEvent = new AuditEvent
        {
            EntityTypeId = typeof(TEntity).GetEntityTypeId(),
            EntityId = entityId,
            AppUserId = DbContext.AppUserId,
            Description = description.Length <= 255 ? description : description[..255],
        };

        foreach (var item in items)
        {
            item.AuditEvent = auditEvent;
            DbContext.AuditItems.Add(item);
        }

        DbContext.AuditEvents.Add(auditEvent);
        return auditEvent;
    }

    private static string FormatValue(object? value)
    {
        return value switch
        {
            null => "",
            DateOnly date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString(),
        } ?? "";
    }
}


public abstract class StaticRepository<TEntity> : BaseRepository<TEntity>, IStaticRepository<TEntity>
    where TEntity : Entity, new()
{
    private static readonly SemaphoreSlim CacheLock = new(1, 1);
    private readonly IMemoryCache _memoryCache;
    private Dictionary<int, TEntity>? _entitiesById;
    private Task<Dictionary<int, TEntity>>? _entitiesByIdTask;

    protected StaticRepository(RushTodoDbContext dbContext, IMemoryCache memoryCache) : base(dbContext)
    {
        _memoryCache = memoryCache;
    }

    public override async Task<TEntity> Add(TEntity entity, string? auditDescription = "")
    {
        entity = await base.Add(entity, auditDescription);
        await DbContext.AfterCommit(() => Refresh(entity.Id));
        return entity;
    }

    public override async Task Delete(int id)
    {
        var entity = await GetForUpdate(id);

        if (entity is IDeletableEntity deletableEntity)
            deletableEntity.IsDeleted = true;
        else
            Set.Remove(entity);

        await Save();
    }

    public override async Task<TEntity> Get(int id)
    {
        var entitiesById = await GetById();
        if (entitiesById.TryGetValue(id, out var entity)) return entity;

        throw new NotFoundException($"{typeof(TEntity).Name} {id} was not found.");
    }

    public virtual async Task<Dictionary<int, TEntity>> GetById()
    {
        if (_entitiesById is not null) return _entitiesById;
        if (_memoryCache.TryGetValue(typeof(TEntity), out Dictionary<int, TEntity>? cachedEntitiesById) && cachedEntitiesById is not null)
        {
            _entitiesById = cachedEntitiesById;
            return cachedEntitiesById;
        }

        await CacheLock.WaitAsync();
        try
        {
            if (_entitiesById is not null) return _entitiesById;
            if (_memoryCache.TryGetValue(typeof(TEntity), out cachedEntitiesById) && cachedEntitiesById is not null)
            {
                _entitiesById = cachedEntitiesById;
                return cachedEntitiesById;
            }

            _entitiesByIdTask ??= LoadById();
        }
        finally
        {
            CacheLock.Release();
        }

        return await _entitiesByIdTask;
    }

    public virtual async Task<TEntity[]> List()
    {
        var entitiesById = await GetById();
        return Sort(entitiesById.Values).ToArray();
    }

    public virtual void Invalidate()
    {
        _entitiesById = null;
        _entitiesByIdTask = null;
        _memoryCache.Remove(typeof(TEntity));
    }

    public async Task Refresh(int id)
    {
        if (_entitiesById is null && !_memoryCache.TryGetValue(typeof(TEntity), out _)) return;

        var refreshedEntity = await Query(query => query.AsNoTracking().FirstOrDefaultAsync(entity => EF.Property<int>(entity, KeyPropertyName) == id));

        await CacheLock.WaitAsync();
        try
        {
            var entitiesById = _entitiesById;
            if (entitiesById is null && !_memoryCache.TryGetValue(typeof(TEntity), out entitiesById)) return;

            var refreshedEntitiesById = new Dictionary<int, TEntity>(entitiesById!);
            if (refreshedEntity is null)
                refreshedEntitiesById.Remove(id);
            else
                refreshedEntitiesById[id] = refreshedEntity;

            SetCachedEntitiesById(refreshedEntitiesById);
        }
        finally
        {
            CacheLock.Release();
        }
    }

    protected virtual IEnumerable<TEntity> Sort(IEnumerable<TEntity> entities) => entities;

    protected override async Task AfterSave(EntityEntry<TEntity>[] changedEntries)
    {
        var changedIds = changedEntries.Select(entry => entry.Entity.Id).Distinct().ToArray();
        foreach (var id in changedIds) await DbContext.AfterCommit(() => Refresh(id));
    }

    private async Task<Dictionary<int, TEntity>> LoadById()
    {
        var entities = await Query(query => query.AsNoTracking().ToArrayAsync());
        var entitiesById = entities.ToDictionary(entity => entity.Id);
        SetCachedEntitiesById(entitiesById);
        return entitiesById;
    }

    private void SetCachedEntitiesById(Dictionary<int, TEntity> entitiesById)
    {
        _entitiesById = entitiesById;
        _memoryCache.Set(typeof(TEntity), entitiesById);
    }
}
