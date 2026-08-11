using RushTodo.Api.Entities;

namespace RushTodo.IntegrationTests;

public class GardenerTestData : TestDataBase<Gardener>
{
    private const int FirstId = 10_001;

    public GardenerTestData()
    {
        AssignIds();
    }

    public TestGardener Bob { get; } = new("Bob", "021 555 0101", "bob@example.test", false);
    public TestGardener Mary { get; } = new("Mary", "021 555 0102", "mary@example.test", false);
    public TestGardener DeletedDave { get; } = new("Deleted Dave", "021 555 0103", null, true);

    private TestGardener[] All => [Bob, Mary, DeletedDave];

    public override async Task Seed(IServiceProvider serviceProvider)
    {
        await Seed(serviceProvider, All.Select(gardener => gardener.CreateEntity()).ToArray());
    }

    private void AssignIds()
    {
        var gardeners = All;
        for (var index = 0; index < gardeners.Length; index++) gardeners[index].Id = FirstId + index;
    }
}


public class TestGardener
{
    internal TestGardener(string name, string phoneNumber, string? emailAddress, bool isDeleted)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress;
        IsDeleted = isDeleted;
    }

    public int Id { get; internal set; }
    public string Name { get; }
    public string PhoneNumber { get; }
    public string? EmailAddress { get; }
    public bool IsDeleted { get; }

    internal Gardener CreateEntity() => new()
    {
        GardenerId = Id,
        Name = Name,
        PhoneNumber = PhoneNumber,
        EmailAddress = EmailAddress,
        IsDeleted = IsDeleted,
    };

}
