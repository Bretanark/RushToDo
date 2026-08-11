using RushTodo.Api.Entities;

namespace RushTodo.IntegrationTests;

public class WorkItemTestData : TestDataBase<WorkItem>
{
    private const int FirstId = 10_001;

    public WorkItemTestData(GardenerTestData gardener)
    {
        BobMowSmithStreet = new()
        {
            Title = "Mow Smith Street",
            StatusId = WorkItemStatusId.Scheduled,
            Address = "10 Smith Street",
            Gardener = gardener.Bob,
            ScheduledDate = new(2025, 8, 10),
        };

        MaryTrimJonesHedge = new()
        {
            Title = "Trim Jones Hedge",
            StatusId = WorkItemStatusId.New,
            Address = "20 Jones Road",
            Gardener = gardener.Mary,
        };

        UnassignedPaintFence = new()
        {
            Title = "Paint Brown Fence",
            StatusId = WorkItemStatusId.New,
            Address = "30 Brown Avenue",
        };

        BobPruneOak = new()
        {
            Title = "Prune Oak Tree",
            StatusId = WorkItemStatusId.Done,
            Address = "40 Green Lane",
            Gardener = gardener.Bob,
            ScheduledDate = new(2025, 8, 1),
            CompletionDate = new(2025, 8, 2),
        };

        MaryWeedGarden = new()
        {
            Title = "Weed Front Garden",
            StatusId = WorkItemStatusId.Cancelled,
            Address = "50 Flower Place",
            Gardener = gardener.Mary,
            ScheduledDate = new(2025, 8, 5),
            CancellationDate = new(2025, 8, 4),
        };

        DeletedRemoveStump = new()
        {
            Title = "Remove Old Stump",
            StatusId = WorkItemStatusId.New,
            Address = "60 Forest Drive",
            Gardener = gardener.Bob,
            ScheduledDate = new(2025, 8, 15),
            IsDeleted = true,
        };

        AssignIds();
    }

    public TestWorkItem BobMowSmithStreet { get; }
    public TestWorkItem MaryTrimJonesHedge { get; }
    public TestWorkItem UnassignedPaintFence { get; }
    public TestWorkItem BobPruneOak { get; }
    public TestWorkItem MaryWeedGarden { get; }
    public TestWorkItem DeletedRemoveStump { get; }
    public TestWorkItem[] DatedWorkItems => [BobPruneOak, MaryWeedGarden, BobMowSmithStreet];

    private TestWorkItem[] All =>
    [
        BobMowSmithStreet,
        MaryTrimJonesHedge,
        UnassignedPaintFence,
        BobPruneOak,
        MaryWeedGarden,
        DeletedRemoveStump,
    ];

    public override async Task Seed(IServiceProvider serviceProvider)
    {
        await Seed(serviceProvider, All.Select(workItem => workItem.CreateEntity()).ToArray());
    }

    private void AssignIds()
    {
        var workItems = All;
        for (var index = 0; index < workItems.Length; index++) workItems[index].Id = FirstId + index;
    }
}


public class TestWorkItem
{
    public int Id { get; internal set; }
    public string Title { get; init; } = "";
    public string? Description { get; init; }
    public WorkItemStatusId StatusId { get; init; }
    public string Address { get; init; } = "";
    public TestGardener? Gardener { get; init; }
    public DateOnly? ScheduledDate { get; init; }
    public DateOnly? CompletionDate { get; init; }
    public DateOnly? CancellationDate { get; init; }
    public bool IsDeleted { get; init; }

    internal WorkItem CreateEntity() => new()
    {
        WorkItemId = Id,
        Title = Title,
        Description = Description,
        StatusId = StatusId,
        Address = Address,
        GardenerId = Gardener?.Id,
        ScheduledDate = ScheduledDate,
        CompletionDate = CompletionDate,
        CancellationDate = CancellationDate,
        IsDeleted = IsDeleted,
    };
}
