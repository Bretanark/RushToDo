using Microsoft.EntityFrameworkCore;
using RushTodo.Api.Entities;
using RushTodo.Api.Services;

namespace RushTodo.Api.Repositories;

public class RushTodoDbContext : DbContext
{
    private readonly IDateTimeService _dateTimeService;

    public RushTodoDbContext(DbContextOptions<RushTodoDbContext> options, IDateTimeService dateTimeService)
        : base(options)
    {
        _dateTimeService = dateTimeService;
    }

    // ReSharper disable UnusedMember.Global
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<AuditItem> AuditItems => Set<AuditItem>();
    public DbSet<EntityType> EntityTypes => Set<EntityType>();
    public DbSet<Gardener> Gardeners => Set<Gardener>();
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();
    public DbSet<WorkItemStatus> WorkItemStatuses => Set<WorkItemStatus>();
    // ReSharper restore UnusedMember.Global

    public override int SaveChanges()
    {
        ApplyUpdateDateTimes();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyUpdateDateTimes();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyUpdateDateTimes();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyUpdateDateTimes();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable(nameof(AppUser));
            entity.HasKey(appUser => appUser.AppUserId);
            entity.Property(appUser => appUser.AppUserId).ValueGeneratedOnAdd();
            entity.Property(appUser => appUser.UpdateDateTime).IsConcurrencyToken();
            entity.HasIndex(appUser => appUser.GoogleSubject).IsUnique().HasFilter($"{nameof(AppUser.GoogleSubject)} IS NOT NULL");
        });

        modelBuilder.Entity<EntityType>(entity =>
        {
            entity.ToTable(nameof(EntityType));
            entity.HasKey(entityType => entityType.EntityTypeId);
            entity.Property(entityType => entityType.EntityTypeId).ValueGeneratedNever();
        });

        modelBuilder.Entity<WorkItemStatus>(entity =>
        {
            entity.ToTable(nameof(WorkItemStatus));
            entity.HasKey(status => status.WorkItemStatusId);
            entity.Property(status => status.WorkItemStatusId).ValueGeneratedNever();
        });

        modelBuilder.Entity<AuditEvent>(entity =>
        {
            entity.ToTable(nameof(AuditEvent));
            entity.HasKey(auditEvent => auditEvent.AuditEventId);
            entity.Property(auditEvent => auditEvent.AuditEventId).ValueGeneratedOnAdd();
            entity.Property(auditEvent => auditEvent.UpdateDateTime).IsConcurrencyToken();
            entity.HasOne<EntityType>().WithMany().HasForeignKey(auditEvent => auditEvent.EntityTypeId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(auditEvent => auditEvent.AppUser).WithMany().HasForeignKey(auditEvent => auditEvent.AppUserId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AuditItem>(entity =>
        {
            entity.ToTable(nameof(AuditItem));
            entity.HasKey(auditItem => auditItem.AuditItemId);
            entity.Property(auditItem => auditItem.AuditItemId).ValueGeneratedOnAdd();
            entity.Property(auditItem => auditItem.UpdateDateTime).IsConcurrencyToken();
            entity.HasOne(auditItem => auditItem.AuditEvent).WithMany(auditEvent => auditEvent.Items).HasForeignKey(auditItem => auditItem.AuditEventId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Gardener>(entity =>
        {
            entity.ToTable(nameof(Gardener));
            entity.HasKey(gardener => gardener.GardenerId);
            entity.Property(gardener => gardener.GardenerId).ValueGeneratedOnAdd();
            entity.Property(gardener => gardener.UpdateDateTime).IsConcurrencyToken();
        });

        modelBuilder.Entity<WorkItem>(entity =>
        {
            entity.ToTable(nameof(WorkItem));
            entity.HasKey(workItem => workItem.WorkItemId);
            entity.Property(workItem => workItem.WorkItemId).ValueGeneratedOnAdd();
            entity.Property(workItem => workItem.ScheduledDate).HasColumnType("DATE");
            entity.Property(workItem => workItem.CompletionDate).HasColumnType("DATE");
            entity.Property(workItem => workItem.CancellationDate).HasColumnType("DATE");
            entity.Property(workItem => workItem.UpdateDateTime).IsConcurrencyToken();
            entity.HasOne<Gardener>().WithMany().HasForeignKey(workItem => workItem.GardenerId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne<WorkItemStatus>().WithMany().HasForeignKey(workItem => workItem.StatusId).OnDelete(DeleteBehavior.NoAction);
        });
    }

    private void ApplyUpdateDateTimes()
    {
        var updateDateTime = _dateTimeService.UtcNow;
        var entries = ChangeTracker.Entries<Entity>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries) entry.Entity.UpdateDateTime = updateDateTime;
    }
}
