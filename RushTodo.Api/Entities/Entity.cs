using System.ComponentModel.DataAnnotations.Schema;

namespace RushTodo.Api.Entities;

public abstract class Entity : IModelEntity
{
    [NotMapped] public abstract int Id { get; }

    [DoNotAudit]
    public DateTime UpdateDateTime { get; set; }

    public virtual string GetText() => $"{GetType().Name} {Id}";
}


public interface IModelEntity
{
    int Id { get; }
    DateTime UpdateDateTime { get; set; }
}
