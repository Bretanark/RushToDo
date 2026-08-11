using RushTodo.Api.Entities;
using RushTodo.Api.Models;
using RushTodo.Api.Repositories;

namespace RushTodo.Api.Services;

public interface IServiceBase<TModel>
    where TModel : Model
{
    Task<TModel> Get(int id);
    TModel New();
    Task<TModel> Save(TModel model, string? auditDescription = "");
}


public abstract class ServiceBase<TEntity, TModel, TRepository> : IServiceBase<TModel>
    where TEntity : Entity, new()
    where TModel : Model, new()
    where TRepository : IBaseRepository<TEntity>
{
    protected ServiceBase(TRepository repository, ITransactionService transactionService)
    {
        Repository = repository;
        TransactionService = transactionService;
    }

    protected TRepository Repository { get; }
    protected ITransactionService TransactionService { get; }

    public virtual TModel New() => new();

    public virtual async Task<TModel> Get(int id)
    {
        var entity = await Repository.Get(id);
        return Map(entity);
    }

    public virtual async Task<LookupItem?> GetLookup(int? id)
    {
        if (id is null or 0) return null;

        var entity = await Repository.Get(id.Value);
        return new(entity.Id, entity.GetText());
    }

    public virtual async Task<TModel> Save(TModel model, string? auditDescription = "")
    {
        return await TransactionService.Run(async () =>
        {
            Validate(model);
            var entity = await Repository.Save(model, auditDescription, entity => Map(model, entity));
            return Map(entity);
        });
    }

    protected static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    protected virtual bool IncludeInLookup(TEntity entity)
        => entity is not IDeletableEntity deletableEntity || !deletableEntity.IsDeleted;

    protected virtual void Validate(TModel model) { }
    protected abstract void Map(TModel model, TEntity entity);
    protected abstract TModel Map(TEntity entity);
}


public abstract class StaticServiceBase<TEntity, TModel, TRepository> : ServiceBase<TEntity, TModel, TRepository>
    where TEntity : Entity, new()
    where TModel : Model, new()
    where TRepository : IStaticRepository<TEntity>
{
    protected StaticServiceBase(TRepository repository, ITransactionService transactionService)
        : base(repository, transactionService) { }

    public virtual async Task<LookupItem[]> GetLookup()
    {
        var entities = await Repository.List();
        return entities
            .Where(IncludeInLookup)
            .Select(entity => new LookupItem(entity.Id, entity.GetText()))
            .ToArray();
    }
}
