namespace RushTodo.IntegrationTests;

public static class TestData
{
    public static GardenerTestData Gardener { get; } = new();
    public static WorkItemTestData WorkItem { get; } = new(Gardener);

    public static async Task Seed(IServiceProvider serviceProvider)
    {
        await Gardener.Seed(serviceProvider);
        await WorkItem.Seed(serviceProvider);
    }
}
