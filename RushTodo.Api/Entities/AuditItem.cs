using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RushTodo.Api.Entities;

public class AuditItem : Entity
{
    public AuditItem() { }

    public AuditItem(string propertyName, string oldValue, string newValue)
    {
        PropertyName = propertyName;
        OldValue = oldValue;
        NewValue = newValue;
    }

    [NotMapped] public override int Id => AuditItemId;

    public int AuditItemId { get; set; }
    public int AuditEventId { get; set; }

    [MaxLength(255)]
    public string PropertyName { get; set; } = "";

    [MaxLength]
    public string? OldValue { get; set; }

    [MaxLength]
    public string? NewValue { get; set; }

    public AuditEvent? AuditEvent { get; set; }
}
