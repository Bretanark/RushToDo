namespace RushTodo.Api.Entities;

public interface IDeletableEntity
{
    bool IsDeleted { get; set; }
}
