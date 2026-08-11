using System.Text.Json.Serialization;
using RushTodo.Api.Entities;

namespace RushTodo.Api.Models;

public abstract class Model : IModelEntity
{
    [JsonIgnore]
    public int Id { get; set; }
    public DateTime UpdateDateTime { get; set; }
}


public abstract class TableRow : Model;
