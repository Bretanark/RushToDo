using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RushTodo.Api.Entities;

public class AuditEvent : Entity
{
    [NotMapped] public override int Id => AuditEventId;

    public int AuditEventId { get; set; }
    public EntityTypeId EntityTypeId { get; set; }
    public int EntityId { get; set; }
    public int AppUserId { get; set; }

    public AppUser? AppUser { get; set; }
    public ICollection<AuditItem> Items { get; set; } = [];

    [MaxLength(255)]
    public string Description { get; set; } = "";
}
