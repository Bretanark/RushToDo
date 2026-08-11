using RushTodo.Api.Entities;

namespace RushTodo.Api.Models;

public class WorkItemModel : Model
{
    public int? WorkItemId
    {
        get => Id == 0 ? null : Id;
        set => Id = value ?? 0;
    }

    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public WorkItemStatusId StatusId { get; set; } = WorkItemStatusId.New;
    public string Address { get; set; } = "";
    public int? GardenerId { get; set; }
    public DateOnly? ScheduledDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public DateOnly? CancellationDate { get; set; }
    public bool IsDeleted { get; set; }
}
