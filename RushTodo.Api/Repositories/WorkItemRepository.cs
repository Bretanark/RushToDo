using Microsoft.EntityFrameworkCore;
using RushTodo.Api.Entities;
using RushTodo.Api.Models;

namespace RushTodo.Api.Repositories;

public interface IWorkItemRepository : IBaseRepository<WorkItem>
{
    Task<WorkItem[]> Search(WorkItemSearchParameters parameters, CancellationToken cancellationToken = default);
}


public class WorkItemRepository : BaseRepository<WorkItem>, IWorkItemRepository
{
    public WorkItemRepository(RushTodoDbContext dbContext) : base(dbContext) { }

    public async Task<WorkItem[]> Search(WorkItemSearchParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = Entities.AsNoTracking();

        if (!parameters.IncludeDeleted) query = query.Where(workItem => !workItem.IsDeleted);

        if (parameters.GardenerIds is { Length: > 0 })
            query = query.Where(workItem => workItem.GardenerId != null && parameters.GardenerIds.Contains(workItem.GardenerId.Value));

        if (parameters.StatusIds is { Length: > 0 })
            query = query.Where(workItem => parameters.StatusIds.Contains(workItem.StatusId));

        if (parameters.ScheduledFrom is not null)
            query = query.Where(workItem => workItem.ScheduledDate >= parameters.ScheduledFrom);

        if (parameters.ScheduledTo is not null)
            query = query.Where(workItem => workItem.ScheduledDate <= parameters.ScheduledTo);

        return await query
            .OrderBy(workItem => workItem.ScheduledDate == null)
            .ThenBy(workItem => workItem.ScheduledDate)
            .ThenBy(workItem => workItem.Title)
            .ThenBy(workItem => workItem.WorkItemId)
            .ToArrayAsync(cancellationToken);
    }
}
