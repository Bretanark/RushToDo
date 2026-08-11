using RushTodo.Api.Entities;

namespace RushTodo.Api.Models;

public abstract class Model : IModelEntity
{
    public int Id { get; set; }
    public DateTime UpdateDateTime { get; set; }
}


public abstract class TableRow : Model;
