using RushTodo.Api.Entities;
using RushTodo.Api.Models;
using RushTodo.Api.Repositories;

namespace RushTodo.Api.Services;

public interface IGardenerService : IServiceBase<GardenerModel>
{
    Task<LookupItem[]> GetLookup();
    Task<LookupItem?> GetLookup(int? id);
}


public class GardenerService : StaticServiceBase<Gardener, GardenerModel, IGardenerRepository>, IGardenerService
{
    public GardenerService(IGardenerRepository gardenerRepository, ITransactionService transactionService)
        : base(gardenerRepository, transactionService) { }

    protected override void Map(GardenerModel model, Gardener gardener)
    {
        gardener.Name = model.Name.Trim();
        gardener.PhoneNumber = model.PhoneNumber.Trim();
        gardener.EmailAddress = Clean(model.EmailAddress);
        gardener.IsDeleted = model.IsDeleted;
    }

    protected override GardenerModel Map(Gardener gardener) => new()
    {
        GardenerId = gardener.GardenerId,
        UpdateDateTime = gardener.UpdateDateTime,
        Name = gardener.Name,
        PhoneNumber = gardener.PhoneNumber,
        EmailAddress = gardener.EmailAddress,
        IsDeleted = gardener.IsDeleted,
    };
}
