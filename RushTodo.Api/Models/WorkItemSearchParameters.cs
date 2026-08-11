using RushTodo.Api.Entities;

namespace RushTodo.Api.Models;

public class WorkItemSearchParameters
{
    public int[]? GardenerIds { get; set; }
    public DateOnly? ScheduledFrom { get; set; }
    public DateOnly? ScheduledTo { get; set; }
    public WorkItemStatusId[]? StatusIds { get; set; }
    public bool IncludeDeleted { get; set; }
}
