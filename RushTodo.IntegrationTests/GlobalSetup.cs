using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RushTodo.Api.Helpers;

namespace RushTodo.IntegrationTests;

[SetUpFixture]
public class GlobalSetup
{
    public static ServiceProvider Services { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.IntegrationTests.json"))
            .AddEnvironmentVariables()
            .Build();
        var connectionString = configuration.GetConnectionString("RushTodo")
            ?? throw new InvalidOperationException("Connection string 'RushTodo' is not configured for integration tests.");

        var services = new ServiceCollection();
        services.AddRushTodoApi(connectionString);
        Services = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });

        await using var scope = Services.CreateAsyncScope();
        await TestData.TestData.Seed(scope.ServiceProvider);
    }

    [OneTimeTearDown]
    public async Task TearDown() => await Services.DisposeAsync();
}
