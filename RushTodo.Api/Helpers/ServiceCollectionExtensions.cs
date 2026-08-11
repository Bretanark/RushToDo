using Microsoft.EntityFrameworkCore;
using RushTodo.Api.Repositories;
using RushTodo.Api.Services;

namespace RushTodo.Api.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRushTodoApi(this IServiceCollection services, string connectionString)
    {
        services.AddMemoryCache();
        services.AddDbContext<RushTodoDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ITransactionService>(serviceProvider => serviceProvider.GetRequiredService<RushTodoDbContext>());
        services.Scan(scan => scan
            .FromAssemblyOf<UserContext>()
            .AddClasses(classes => classes.InNamespaces(
                typeof(UserContext).Namespace!,
                typeof(GardenerRepository).Namespace!))
            .AsMatchingInterface()
            .WithScopedLifetime());

        return services;
    }
}
