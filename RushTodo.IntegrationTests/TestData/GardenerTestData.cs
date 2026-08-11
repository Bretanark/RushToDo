using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RushTodo.Api.Entities;
using RushTodo.Api.Repositories;

namespace RushTodo.IntegrationTests;

public class GardenerTestData
{
    private const int FirstId = 10_001;

    public TestGardener Bob { get; } = new("Bob", "021 555 0101", "bob@example.test", false);
    public TestGardener Mary { get; } = new("Mary", "021 555 0102", "mary@example.test", false);
    public TestGardener DeletedDave { get; } = new("Deleted Dave", "021 555 0103", null, true);

    private TestGardener[] All => [Bob, Mary, DeletedDave];

    public async Task Seed(IServiceProvider serviceProvider)
    {
        AssignIds();

        var dbContext = serviceProvider.GetRequiredService<RushTodoDbContext>();
        var ids = All.Select(gardener => gardener.Id).ToArray();
        var existingById = await dbContext.Gardeners
            .AsNoTracking()
            .Where(gardener => ids.Contains(gardener.GardenerId))
            .ToDictionaryAsync(gardener => gardener.GardenerId);

        var missing = All
            .Where(gardener => !existingById.ContainsKey(gardener.Id))
            .Select(gardener => gardener.CreateEntity())
            .ToArray();
        if (missing.Length == 0) return;

        dbContext.Gardeners.AddRange(missing);
        await dbContext.Database.OpenConnectionAsync();
        var identityInsertEnabled = false;
        try
        {
            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Gardener ON");
            identityInsertEnabled = true;
            await dbContext.SaveChangesAsync();
        }
        finally
        {
            if (identityInsertEnabled) await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Gardener OFF");
            await dbContext.Database.CloseConnectionAsync();
        }
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
