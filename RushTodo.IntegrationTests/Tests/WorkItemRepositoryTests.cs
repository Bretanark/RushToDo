using NUnit.Framework;
using RushTodo.Api.Entities;
using RushTodo.Api.Repositories;

namespace RushTodo.IntegrationTests;

[TestFixture]
public class WorkItemRepositoryTests : IntegrationTestBase<IWorkItemRepository>
{
    [Test]
    public async Task CreateReadUpdate()
    {
        var created = await RunInTransaction(repository =>
            repository.Save((IModelEntity?)null, "", workItem =>
            {
                workItem.Title = "WorkItemRepositoryTests CreateReadUpdate";
                workItem.Description = "Created by WorkItemRepositoryTests CreateReadUpdate";
                workItem.StatusId = WorkItemStatusId.New;
                workItem.Address = "1 Integration Lane";
                workItem.GardenerId = TestData.Gardener.Bob.Id;
            }));

        var read = await Run(repository => repository.Get(created.Id));
        await RunInTransaction(repository => repository.Save(read, "",
            workItem => workItem.Title = "WorkItemRepositoryTests CreateReadUpdate - Updated"));
        var actual = await Run(repository => repository.Get(created.Id));

        const string expected = """
            Title,Description,StatusId,Address,GardenerId,IsDeleted
            WorkItemRepositoryTests CreateReadUpdate - Updated,Created by WorkItemRepositoryTests CreateReadUpdate,New,1 Integration Lane,10001,FALSE
            """;
        CsvAssert.Contains(expected, [actual]);
        Assert.That(actual.UpdateDateTime, Is.GreaterThan(read.UpdateDateTime));
    }
}
