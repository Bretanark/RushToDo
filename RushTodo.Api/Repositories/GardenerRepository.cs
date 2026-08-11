using Microsoft.Extensions.Caching.Memory;
using RushTodo.Api.Entities;

namespace RushTodo.Api.Repositories;

public interface IGardenerRepository : IStaticRepository<Gardener>;


public class GardenerRepository : StaticRepository<Gardener>, IGardenerRepository
{
    public GardenerRepository(RushTodoDbContext dbContext, IMemoryCache memoryCache)
        : base(dbContext, memoryCache) { }

    protected override IEnumerable<Gardener> Sort(IEnumerable<Gardener> entities) => entities.OrderBy(gardener => gardener.Name);
}
