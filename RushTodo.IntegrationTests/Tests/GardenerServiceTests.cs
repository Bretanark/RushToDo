using NUnit.Framework;
using RushTodo.Api.Services;

namespace RushTodo.IntegrationTests;

[TestFixture]
public class GardenerServiceTests : IntegrationTestBase<IGardenerService>
{
    [Test]
    public async Task GetLookupReturnsActiveGardeners()
    {
        var actual = await Run(service => service.GetLookup());

        const string expected = """
            Id,Text
            10001,Bob
            10002,Mary
            """;
        CsvAssert.Contains(expected, actual);
        Assert.That(actual.Select(gardener => gardener.Id), Does.Not.Contain(TestData.Gardener.DeletedDave.Id));
    }
}
