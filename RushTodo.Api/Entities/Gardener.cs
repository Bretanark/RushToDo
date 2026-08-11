using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RushTodo.Api.Entities;

public class Gardener : Entity, IDeletableEntity
{
    [NotMapped] public override int Id => GardenerId;

    public int GardenerId { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = "";

    [MaxLength(50)]
    public string PhoneNumber { get; set; } = "";

    [MaxLength(255)]
    public string? EmailAddress { get; set; }

    public bool IsDeleted { get; set; }

    public override string GetText() => Name;
}
