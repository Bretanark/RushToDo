using RushTodo.Api.Entities;
using RushTodo.Api.Models;

namespace RushTodo.Api.Validators;

public class WorkItemValidator(WorkItemModel workItem) : ValidatorBase<WorkItemModel>(workItem)
{
    public TextValidator Title => new()
    {
        Label = "Title",
        GetValue = () => Model.Title,
        MaxLength = 255,
        Validation = value => ValidateRequiredText(value, 255),
    };

    public TextValidator Description => new()
    {
        Label = "Description",
        GetValue = () => Model.Description,
    };

    public TextValidator Address => new()
    {
        Label = "Address",
        GetValue = () => Model.Address,
        MaxLength = 255,
        Validation = value => ValidateRequiredText(value, 255),
    };

    public DateValidator ScheduledDate => new()
    {
        Label = "Scheduled date",
        GetValue = () => Model.ScheduledDate,
        IsVisible = Model.StatusId == WorkItemStatusId.Scheduled,
        Validation = value => value is null ? "[Label] is required when status is Scheduled" : null,
    };

    public DateValidator CompletionDate => new()
    {
        Label = "Completion date",
        GetValue = () => Model.CompletionDate,
        IsVisible = Model.StatusId == WorkItemStatusId.Done,
        Validation = value => value is null ? "[Label] is required when status is Done" : null,
    };

    public DateValidator CancellationDate => new()
    {
        Label = "Cancellation date",
        GetValue = () => Model.CancellationDate,
        IsVisible = Model.StatusId == WorkItemStatusId.Cancelled,
        Validation = value => value is null ? "[Label] is required when status is Cancelled" : null,
    };

    public override string? Validate()
        => Title.Validate()
           ?? Address.Validate()
           ?? ScheduledDate.Validate()
           ?? CompletionDate.Validate()
           ?? CancellationDate.Validate();

    private static string? ValidateRequiredText(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return "[Label] is required";
        return value.Length > maxLength ? $"[Label] cannot exceed {maxLength} characters" : null;
    }
}
