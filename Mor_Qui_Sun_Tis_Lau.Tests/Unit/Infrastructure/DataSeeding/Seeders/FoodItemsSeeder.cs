using MediatR;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Seeders;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Infrastructure.DataSeeding.Seeders;

public class FoodItemsSeederTests
{
    private readonly Mock<IMediator> _mockMediator = new();

    [Fact]
    public async Task SeedData_ShouldSeedFoodItemsAndNotSetOfProductCreatedEvents_WhenThereIsZeroFoodItemsInDBAndEnvironmentIsNotDevelopment()
    {
        using var db = DbTest.CreateContext();

        await FoodItemsSeeder.SeedData(db, _mockMediator.Object, false);

        Assert.True(db.FoodItems.Any());
        Assert.Equal(12, db.FoodItems.Count());

        _mockMediator.Verify(m => m.Publish(It.IsAny<ProductCreated>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task SeedData_ShouldSeedFoodItemsAndSetOfProductCreatedEvents_WhenThereIsZeroFoodItemsInDBAndEnvironmentIsDevelopment()
    {
        using var db = DbTest.CreateContext();

        await FoodItemsSeeder.SeedData(db, _mockMediator.Object, true);

        Assert.True(db.FoodItems.Any());
        Assert.Equal(12, db.FoodItems.Count());

        _mockMediator.Verify(m => m.Publish(It.IsAny<ProductCreated>(), CancellationToken.None), Times.Exactly(12));
    }

    [Fact]
    public async Task SeedData_ShouldNotSeedFoodItems_WhenThereIsFoodItemsInDB()
    {
        using var db = DbTest.CreateContext();

        var foodItem = new FoodItem("name", "description", 10m, "imageLink");
        db.FoodItems.Add(foodItem);
        db.SaveChanges();

        await FoodItemsSeeder.SeedData(db, _mockMediator.Object, true);

        Assert.True(db.FoodItems.Any());
        Assert.Equal(1, db.FoodItems.Count());

        _mockMediator.Verify(m => m.Publish(It.IsAny<ProductCreated>(), CancellationToken.None), Times.Never);
    }
}