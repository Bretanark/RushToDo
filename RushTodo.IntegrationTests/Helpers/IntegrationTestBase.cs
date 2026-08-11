using Microsoft.Extensions.DependencyInjection;
using RushTodo.Api.Services;

namespace RushTodo.IntegrationTests;

public abstract class IntegrationTestBase<TSubject>
    where TSubject : notnull
{
    protected static async Task<TResult> Run<TResult>(Func<TSubject, Task<TResult>> action)
    {
        await using var scope = GlobalSetup.Services.CreateAsyncScope();
        var subject = scope.ServiceProvider.GetRequiredService<TSubject>();
        return await action(subject);
    }

    protected static async Task<TResult> RunInTransaction<TResult>(Func<TSubject, Task<TResult>> action)
    {
        await using var scope = GlobalSetup.Services.CreateAsyncScope();
        var subject = scope.ServiceProvider.GetRequiredService<TSubject>();
        var transaction = scope.ServiceProvider.GetRequiredService<ITransactionService>();
        return await transaction.Run(() => action(subject));
    }
}
