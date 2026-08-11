using Microsoft.EntityFrameworkCore;
using RushTodo.Api.Repositories;
using RushTodo.Api.Services;

namespace RushTodo.Api.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRushTodoApi(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddDbContext<RushTodoDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }
}
