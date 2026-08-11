using NUnit.Framework;
using RushTodo.Api.Repositories;

namespace RushTodo.IntegrationTests;

[TestFixture]
public class GardenerRepositoryTests : IntegrationTestBase<IGardenerRepository>
{
    [Test]
    public async Task ListIncludesTestData()
    {
        var actual = await Run(repository => repository.List());

        const string expected = """
            Id,Name,PhoneNumber,EmailAddress,IsDeleted
            10001,Bob,021 555 0101,bob@example.test,FALSE
            10002,Mary,021 555 0102,mary@example.test,FALSE
            10003,Deleted Dave,021 555 0103,,TRUE
            """;
        CsvAssert.Contains(expected, actual);
    }
}
