using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RushTodo.Api.Entities;

public class WorkItem : Entity, IDeletableEntity
{
    [NotMapped] public override int Id => WorkItemId;

    public int WorkItemId { get; set; }

    [MaxLength(255)]
    public string Title { get; set; } = "";

    [MaxLength]
    [Column(TypeName = "varchar(max)")]
    public string? Description { get; set; }

    public WorkItemStatusId StatusId { get; set; }

    [MaxLength(255)]
    public string Address { get; set; } = "";

    public int? GardenerId { get; set; }
    public DateOnly? ScheduledDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public DateOnly? CancellationDate { get; set; }

    // TODO: WorkItems should not be deletable. Remove IsDeleted and its soft-delete plumbing.
    public bool IsDeleted { get; set; }

    public override string GetText() => Title;
}
