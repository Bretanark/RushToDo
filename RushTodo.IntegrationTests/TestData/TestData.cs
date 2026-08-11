namespace RushTodo.IntegrationTests.TestData;

public static class TestData
{
    public static GardenerTestData Gardener { get; } = new();

    public static async Task Seed(IServiceProvider serviceProvider)
    {
        await Gardener.Seed(serviceProvider);
    }
}
