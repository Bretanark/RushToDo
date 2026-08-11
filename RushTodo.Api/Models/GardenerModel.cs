namespace RushTodo.Api.Models;

public class GardenerModel : Model
{
    public int? GardenerId
    {
        get => Id == 0 ? null : Id;
        set => Id = value ?? 0;
    }

    public string Name { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string? EmailAddress { get; set; }
    public bool IsDeleted { get; set; }
}
