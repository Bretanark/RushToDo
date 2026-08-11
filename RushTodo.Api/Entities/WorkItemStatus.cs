using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RushTodo.Api.Entities;

public class WorkItemStatus : Enummy
{
    [NotMapped] public override int Id => (int)WorkItemStatusId;

    public WorkItemStatusId WorkItemStatusId { get; private set; }

    [MaxLength(255)]
    public string WorkItemStatusName { get; private set; } = "";

    public override string GetText() => WorkItemStatusName;
}


public enum WorkItemStatusId
{
    New = 1,
    Scheduled = 2,
    Done = 3,
    Cancelled = 4,
}
