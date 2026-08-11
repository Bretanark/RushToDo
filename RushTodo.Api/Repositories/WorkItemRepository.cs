using RushTodo.Api.Entities;

namespace RushTodo.Api.Repositories;

public interface IWorkItemRepository : IBaseRepository<WorkItem>;


public class WorkItemRepository : BaseRepository<WorkItem>, IWorkItemRepository
{
    public WorkItemRepository(RushTodoDbContext dbContext) : base(dbContext) { }
}
