using System.ComponentModel.DataAnnotations.Schema;

namespace RushTodo.Api.Entities;

public abstract class Enummy
{
    [NotMapped] public abstract int Id { get; }

    public abstract string GetText();

    public override string ToString() => GetText();
}
