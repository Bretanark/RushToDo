using Moq;
using NUnit.Framework;
using RushTodo.Api.Entities;
using RushTodo.Api.Models;
using RushTodo.Api.Repositories;
using RushTodo.Api.Services;

namespace RushTodo.UnitTests;

[TestFixture]
public class GardenerServiceUnitTests
{
    [Test]
    public async Task GetLookupReturnsActiveGardeners()
    {
        Gardener[] gardeners =
        [
            new() { GardenerId = 1, Name = "Bob" },
            new() { GardenerId = 2, Name = "Deleted Dave", IsDeleted = true },
            new() { GardenerId = 3, Name = "Mary" },
        ];
        var gardenerRepository = new Mock<IGardenerRepository>(MockBehavior.Strict);
        gardenerRepository.Setup(repository => repository.List()).ReturnsAsync(gardeners);
        var transactionService = new Mock<ITransactionService>(MockBehavior.Strict);
        var service = new GardenerService(gardenerRepository.Object, transactionService.Object);

        var actual = await service.GetLookup();

        LookupItem[] expected = [new(1, "Bob"), new(3, "Mary")];
        Assert.That(actual, Is.EqualTo(expected));
        gardenerRepository.Verify(repository => repository.List(), Times.Once);
        gardenerRepository.VerifyNoOtherCalls();
        transactionService.VerifyNoOtherCalls();
    }
}
