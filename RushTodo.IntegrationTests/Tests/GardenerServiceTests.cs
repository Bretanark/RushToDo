using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RushTodo.Api.Services;

namespace RushTodo.IntegrationTests;

[TestFixture]
public class GardenerServiceTests
{
    [Test]
    public async Task GetLookupReturnsActiveGardeners()
    {
        await using var scope = GlobalSetup.Services.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<IGardenerService>();

        var actual = await service.GetLookup();

        const string expected = """
            Id,Text
            10001,Bob
            10002,Mary
            """;
        CsvAssert.Contains(expected, actual);
        Assert.That(actual.Select(gardener => gardener.Id), Does.Not.Contain(TestData.Gardener.DeletedDave.Id));
    }
}
