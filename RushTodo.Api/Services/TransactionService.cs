namespace RushTodo.Api.Services;

public interface ITransactionService
{
    Task AfterCommit(Func<Task> action);
    Task Run(Func<Task> action);
    Task<TResult> Run<TResult>(Func<Task<TResult>> action);
    DateTime UpdateDateTime { get; }
}
