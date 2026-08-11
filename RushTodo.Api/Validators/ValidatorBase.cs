using RushTodo.Api.Helpers;
using RushTodo.Api.Models;

namespace RushTodo.Api.Validators;

public abstract class ValidatorBase
{
    public string Label { get; init; } = "This field";
    public bool IsEditable { get; init; } = true;
    public bool IsVisible { get; init; } = true;

    public virtual string? Validate() => null;

    public void Assert()
    {
        var validation = Validate();
        if (validation is not null) throw new ValidationException(validation);
    }
}


public abstract class ValidatorBase<TModel>(TModel model) : ValidatorBase
    where TModel : Model
{
    protected TModel Model { get; } = model;
}


public class ValueValidator<T> : ValidatorBase
{
    public Func<T>? GetValue { get; init; }
    public Func<T, string?>? Validation { get; init; }

    public override string? Validate()
    {
        if (!IsVisible || GetValue is null) return null;

        var value = GetValue();
        return Validate(value)?.Replace("[Label]", Label);
    }

    public string? Validate(T value)
        => IsVisible ? Validation?.Invoke(value)?.Replace("[Label]", Label) : null;
}


public class DateValidator : ValueValidator<DateOnly?>
{
    public DateOnly? Min { get; init; }
    public DateOnly? Max { get; init; }
}


public class TextValidator : ValueValidator<string?>
{
    public int? MaxLength { get; init; }
}
