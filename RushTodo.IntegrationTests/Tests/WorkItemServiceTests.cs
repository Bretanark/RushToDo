using NUnit.Framework;
using RushTodo.Api.Entities;
using RushTodo.Api.Models;
using RushTodo.Api.Services;

namespace RushTodo.IntegrationTests;

[TestFixture]
public class WorkItemServiceTests : IntegrationTestBase<IWorkItemService>
{
    [Test]
    public async Task SearchReturnsActiveWorkItems()
    {
        var actual = await Run(service => service.Search(new()));

        const string expected = """
            Title,StatusId,GardenerId,ScheduledDate,CompletionDate,CancellationDate
            Prune Oak Tree,Done,10001,2025-08-01,2025-08-02,
            Weed Front Garden,Cancelled,10002,2025-08-05,,2025-08-04
            Mow Smith Street,Scheduled,10001,2025-08-10,,
            Paint Brown Fence,New,,,,
            Trim Jones Hedge,New,10002,,,
            """;
        CsvAssert.Contains(expected, actual);
        Assert.That(actual, Has.All.Property(nameof(WorkItemModel.IsDeleted)).False);
        Assert.That(actual.Select(workItem => workItem.Id), Does.Not.Contain(TestData.WorkItem.DeletedRemoveStump.Id));
    }

    [TestCaseSource(nameof(SearchFilters_Cases))]
    public async Task SearchFilters(WorkItemSearchParameters parameters, int expectedSeedCount, string expected)
    {
        var actual = await Run(service => service.Search(parameters));

        CsvAssert.Contains(expected, actual);
        var datedTestDataTitles = TestData.WorkItem.DatedWorkItems.Select(workItem => workItem.Title).ToArray();
        Assert.That(actual.Count(workItem => datedTestDataTitles.Contains(workItem.Title)), Is.EqualTo(expectedSeedCount));
    }

    private static IEnumerable<TestCaseData> SearchFilters_Cases()
    {
        yield return new TestCaseData(
                new WorkItemSearchParameters
                {
                    ScheduledFrom = new(2025, 8, 1),
                    ScheduledTo = new(2025, 8, 5),
                },
                2,
                """
                Title,StatusId,GardenerId,ScheduledDate
                Prune Oak Tree,Done,10001,2025-08-01
                Weed Front Garden,Cancelled,10002,2025-08-05
                """)
            .SetName("Search_DateRange_IncludesTwo");

        yield return new TestCaseData(
                new WorkItemSearchParameters
                {
                    ScheduledFrom = new(2025, 8, 1),
                    ScheduledTo = new(2025, 8, 10),
                },
                3,
                """
                Title,StatusId,GardenerId,ScheduledDate
                Prune Oak Tree,Done,10001,2025-08-01
                Weed Front Garden,Cancelled,10002,2025-08-05
                Mow Smith Street,Scheduled,10001,2025-08-10
                """)
            .SetName("Search_WiderDateRange_IncludesThird");

        yield return new TestCaseData(
                new WorkItemSearchParameters
                {
                    GardenerIds = [TestData.Gardener.Bob.Id],
                    ScheduledFrom = new(2025, 8, 1),
                    ScheduledTo = new(2025, 8, 10),
                },
                2,
                """
                Title,StatusId,GardenerId,ScheduledDate
                Prune Oak Tree,Done,10001,2025-08-01
                Mow Smith Street,Scheduled,10001,2025-08-10
                """)
            .SetName("Search_Gardener_IncludesTwo");

        yield return new TestCaseData(
                new WorkItemSearchParameters
                {
                    GardenerIds = [TestData.Gardener.Bob.Id, TestData.Gardener.Mary.Id],
                    ScheduledFrom = new(2025, 8, 1),
                    ScheduledTo = new(2025, 8, 10),
                },
                3,
                """
                Title,StatusId,GardenerId,ScheduledDate
                Prune Oak Tree,Done,10001,2025-08-01
                Weed Front Garden,Cancelled,10002,2025-08-05
                Mow Smith Street,Scheduled,10001,2025-08-10
                """)
            .SetName("Search_MoreGardeners_IncludesThird");

        yield return new TestCaseData(
                new WorkItemSearchParameters
                {
                    StatusIds = [WorkItemStatusId.Scheduled],
                    ScheduledFrom = new(2025, 8, 1),
                    ScheduledTo = new(2025, 8, 10),
                },
                1,
                """
                Title,StatusId,GardenerId,ScheduledDate
                Mow Smith Street,Scheduled,10001,2025-08-10
                """)
            .SetName("Search_Status_IncludesOne");
    }

    [Test]
    public async Task SaveGetUpdate()
    {
        var model = new WorkItemModel
        {
            Title = "WorkItemServiceTests SaveGetUpdate",
            Description = "Created by WorkItemServiceTests SaveGetUpdate",
            Address = "2 Integration Lane",
            GardenerId = TestData.Gardener.Mary.Id,
            ScheduledDate = new(2026, 8, 12),
        };
        var created = await Run(service => service.Save(model));
        Assert.That(created.StatusId, Is.EqualTo(WorkItemStatusId.Scheduled));

        var read = await Run(service => service.Get(created.Id));
        read.Title = "WorkItemServiceTests SaveGetUpdate - Updated";
        read.ScheduledDate = null;
        await Run(service => service.Save(read));
        var actual = await Run(service => service.Get(created.Id));

        const string expected = """
            Title,Description,StatusId,Address,GardenerId,ScheduledDate,CompletionDate,CancellationDate,IsDeleted
            WorkItemServiceTests SaveGetUpdate - Updated,Created by WorkItemServiceTests SaveGetUpdate,New,2 Integration Lane,10002,,,,FALSE
            """;
        CsvAssert.Contains(expected, [actual]);
        Assert.That(actual.UpdateDateTime, Is.GreaterThan(read.UpdateDateTime));
    }

}
