using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RushTodo.Api.Entities;
using RushTodo.Api.Repositories;

namespace RushTodo.IntegrationTests;

public abstract class TestDataBase<TEntity>
    where TEntity : Entity
{
    public abstract Task Seed(IServiceProvider serviceProvider);

    protected static async Task Seed(IServiceProvider serviceProvider, TEntity[] entities)
    {
        var dbContext = serviceProvider.GetRequiredService<RushTodoDbContext>();
        var set = dbContext.Set<TEntity>();
        var missing = new List<TEntity>();
        foreach (var entity in entities)
        {
            if (await set.FindAsync(entity.Id) is null) missing.Add(entity);
        }

        if (missing.Count == 0) return;

        set.AddRange(missing);
        var connectionWasClosed = dbContext.Database.GetDbConnection().State == ConnectionState.Closed;
        if (connectionWasClosed) await dbContext.Database.OpenConnectionAsync();

        var tableName = dbContext.Model.FindEntityType(typeof(TEntity))?.GetTableName()
            ?? throw new InvalidOperationException($"{typeof(TEntity).Name} is not mapped to a table.");
        var tableIdentifier = $"dbo.[{tableName.Replace("]", "]]", StringComparison.Ordinal)}]";
        var enableIdentityInsert = $"SET IDENTITY_INSERT {tableIdentifier} ON";
        var disableIdentityInsert = $"SET IDENTITY_INSERT {tableIdentifier} OFF";
        var identityInsertEnabled = false;
        try
        {
            await dbContext.Database.ExecuteSqlRawAsync(enableIdentityInsert);
            identityInsertEnabled = true;
            await dbContext.SaveChangesAsync();
        }
        finally
        {
            if (identityInsertEnabled)
                await dbContext.Database.ExecuteSqlRawAsync(disableIdentityInsert);
            if (connectionWasClosed) await dbContext.Database.CloseConnectionAsync();
        }
    }
}
