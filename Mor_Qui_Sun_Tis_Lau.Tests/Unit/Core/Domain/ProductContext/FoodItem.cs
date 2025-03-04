using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.ProductContext;

public class FoodItemTests
{
    [Fact]
    public void ConstructorWithValues_ShouldAssignValuesAndNotAddEvents()
    {
        var name = "firstName";
        var description = "description";
        var price = 100m;
        var imageLink = "imageLink";

        var foodItem = new FoodItem(name, description, price, imageLink);

        Assert.Equal(Guid.Empty, foodItem.Id);
        Assert.Equal(name, foodItem.Name);
        Assert.Equal(description, foodItem.Description);
        Assert.Equal(price, foodItem.Price);
        Assert.Equal(imageLink, foodItem.ImageLink);
        Assert.True(string.IsNullOrWhiteSpace(foodItem.Stripe_productId));

        // Same name or price should not trigger event
        foodItem.Price = price;
        foodItem.Name = name;
        Assert.Empty(foodItem.Events);
    }

    [Fact]
    public void ConstructorWithEmptyImageLink_ImageLinkShouldBeAssignedFiller()
    {
        var name = "firstName";
        var description = "description";
        var price = 100m;
        var imageLink = string.Empty;

        var foodItem = new FoodItem(name, description, price, imageLink);

        Assert.Equal(Guid.Empty, foodItem.Id);
        Assert.Equal(name, foodItem.Name);
        Assert.Equal(description, foodItem.Description);
        Assert.Equal(price, foodItem.Price);
        Assert.Equal("filler.jpg", foodItem.ImageLink);

        Assert.Empty(foodItem.Events);
    }

    [Fact]
    public void NameOnchange_ShouldAddEventAndAssignValue()
    {
        var oldName = "oldName";
        var newName = "newName";

        var foodItem = new FoodItem(oldName, "description", 100m, "imageLink")
        {
            Name = newName
        };

        Assert.NotEmpty(foodItem.Events);
        Assert.IsType<FoodItemNameChanged>(foodItem.Events[0]);
        Assert.Equal(newName, foodItem.Name);
    }

    [Fact]
    public void PriceOnchange_ShouldAddEventAndAssignValue()
    {
        var oldPrice = 50m;
        var newPrice = 80m;

        var foodItem = new FoodItem("name", "description", oldPrice, "imageLink")
        {
            Price = newPrice
        };

        Assert.NotEmpty(foodItem.Events);
        Assert.IsType<FoodItemPriceChanged>(foodItem.Events[0]);
        Assert.Equal(newPrice, foodItem.Price);
    }

    [Fact]
    public void EditFoodItem_ShouldUpdateValues()
    {
        var foodItem = new FoodItem("name", "description", 10m, "imageLink");

        var newName = "newName";
        var newDescription = "newDescription";
        var newPrice = 12m;

        foodItem.EditFoodItem(newName, newDescription, newPrice, null);

        Assert.Equal(newName, foodItem.Name);
        Assert.Equal(newDescription, foodItem.Description);
        Assert.Equal(newPrice, foodItem.Price);
        Assert.Equal("filler.jpg", foodItem.ImageLink);
        Assert.Equal(2, foodItem.Events.Count);
    }
}