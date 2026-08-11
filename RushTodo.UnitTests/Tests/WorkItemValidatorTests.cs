using NUnit.Framework;
using RushTodo.Api.Entities;
using RushTodo.Api.Helpers;
using RushTodo.Api.Models;
using RushTodo.Api.Validators;

namespace RushTodo.UnitTests;

[TestFixture]
public class WorkItemValidatorTests
{
    [Test]
    public void TitleIsRequired()
    {
        var model = ValidWorkItem();
        model.Title = " ";

        Assert.That(new WorkItemValidator(model).Validate(), Is.EqualTo("Title is required"));
    }

    [Test]
    public void TitleCannotExceedMaximumLength()
    {
        var model = ValidWorkItem();
        model.Title = new('x', 256);

        Assert.That(new WorkItemValidator(model).Validate(), Is.EqualTo("Title cannot exceed 255 characters"));
    }

    [Test]
    public void AddressIsRequired()
    {
        var model = ValidWorkItem();
        model.Address = "";

        Assert.That(new WorkItemValidator(model).Validate(), Is.EqualTo("Address is required"));
    }

    [Test]
    public void AddressCannotExceedMaximumLength()
    {
        var model = ValidWorkItem();
        model.Address = new('x', 256);

        Assert.That(new WorkItemValidator(model).Validate(), Is.EqualTo("Address cannot exceed 255 characters"));
    }

    [TestCase(WorkItemStatusId.Scheduled, "Scheduled date is required when status is Scheduled")]
    [TestCase(WorkItemStatusId.Done, "Completion date is required when status is Done")]
    [TestCase(WorkItemStatusId.Cancelled, "Cancellation date is required when status is Cancelled")]
    public void StatusRequiresItsBusinessDate(WorkItemStatusId statusId, string expected)
    {
        var model = ValidWorkItem();
        model.StatusId = statusId;
        model.ScheduledDate = null;
        model.CompletionDate = null;
        model.CancellationDate = null;

        Assert.That(new WorkItemValidator(model).Validate(), Is.EqualTo(expected));
    }

    [TestCase(WorkItemStatusId.New)]
    [TestCase(WorkItemStatusId.Scheduled)]
    [TestCase(WorkItemStatusId.Done)]
    [TestCase(WorkItemStatusId.Cancelled)]
    public void ValidWorkItemPasses(WorkItemStatusId statusId)
    {
        var model = ValidWorkItem();
        model.StatusId = statusId;

        Assert.That(new WorkItemValidator(model).Validate(), Is.Null);
    }

    [Test]
    public void AssertThrowsValidationException()
    {
        var model = ValidWorkItem();
        model.Title = "";

        var exception = Assert.Throws<ValidationException>(() => new WorkItemValidator(model).Assert());
        Assert.That(exception?.Message, Is.EqualTo("Title is required"));
    }

    private static WorkItemModel ValidWorkItem() => new()
    {
        Title = "Mow Smith Street",
        Address = "10 Smith Street",
        ScheduledDate = new(2025, 8, 10),
        CompletionDate = new(2025, 8, 10),
        CancellationDate = new(2025, 8, 10),
    };
}
