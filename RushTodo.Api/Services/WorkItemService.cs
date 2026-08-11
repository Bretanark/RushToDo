using RushTodo.Api.Entities;
using RushTodo.Api.Models;
using RushTodo.Api.Repositories;
using RushTodo.Api.Validators;

namespace RushTodo.Api.Services;

public interface IWorkItemService : IServiceBase<WorkItemModel>
{
    Task<WorkItemModel[]> Search(WorkItemSearchParameters parameters, CancellationToken cancellationToken = default);
}


public class WorkItemService : ServiceBase<WorkItem, WorkItemModel, IWorkItemRepository>, IWorkItemService
{
    public WorkItemService(IWorkItemRepository workItemRepository, ITransactionService transactionService)
        : base(workItemRepository, transactionService) { }

    public override Task<WorkItemModel> Save(WorkItemModel model, string? auditDescription = "")
    {
        model.StatusId = GetStatusId(model);
        return base.Save(model, auditDescription);
    }

    public async Task<WorkItemModel[]> Search(WorkItemSearchParameters parameters, CancellationToken cancellationToken = default)
    {
        var workItems = await Repository.Search(parameters, cancellationToken);
        return workItems.Select(Map).ToArray();
    }

    protected override WorkItemValidator GetValidator(WorkItemModel model) => new(model);

    private static WorkItemStatusId GetStatusId(WorkItemModel model)
    {
        if (model.CancellationDate.HasValue) return WorkItemStatusId.Cancelled;
        if (model.CompletionDate.HasValue) return WorkItemStatusId.Done;
        return model.ScheduledDate.HasValue ? WorkItemStatusId.Scheduled : WorkItemStatusId.New;
    }

    protected override void Map(WorkItemModel model, WorkItem workItem)
    {
        workItem.Title = model.Title.Trim();
        workItem.Description = Clean(model.Description);
        workItem.StatusId = model.StatusId;
        workItem.Address = model.Address.Trim();
        workItem.GardenerId = model.GardenerId;
        workItem.ScheduledDate = model.ScheduledDate;
        workItem.CompletionDate = model.CompletionDate;
        workItem.CancellationDate = model.CancellationDate;
        workItem.IsDeleted = model.IsDeleted;
    }

    protected override WorkItemModel Map(WorkItem workItem) => new()
    {
        WorkItemId = workItem.WorkItemId,
        UpdateDateTime = workItem.UpdateDateTime,
        Title = workItem.Title,
        Description = workItem.Description,
        StatusId = workItem.StatusId,
        Address = workItem.Address,
        GardenerId = workItem.GardenerId,
        ScheduledDate = workItem.ScheduledDate,
        CompletionDate = workItem.CompletionDate,
        CancellationDate = workItem.CancellationDate,
        IsDeleted = workItem.IsDeleted,
    };
}
