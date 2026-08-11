using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RushTodo.Api.Entities;

public class AppUser : Entity
{
    [NotMapped] public override int Id => AppUserId;

    public int AppUserId { get; set; }

    [MaxLength(255)]
    public string? EmailAddress { get; set; }

    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(255)]
    public string? GoogleSubject { get; set; }

    public override string GetText() => Name ?? EmailAddress ?? base.GetText();
}
