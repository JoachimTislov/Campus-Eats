using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.ProductContext;

public class EventsTests
{
    [Fact]
    public void FoodItemNameChangedEvent()
    {
        var itemId = Guid.NewGuid();
        var oldName = "oldName";
        var newName = "newName";

        var foodItemNameChanged = new FoodItemNameChanged(itemId, oldName, newName);

        Assert.Equal(itemId, foodItemNameChanged.ItemId);
        Assert.Equal(oldName, foodItemNameChanged.OldName);
        Assert.Equal(newName, foodItemNameChanged.NewName);
    }

    [Fact]
    public void FoodPriceNameChangedEvent()
    {
        var itemId = Guid.NewGuid();
        var oldPrice = 50m;
        var newPrice = 80m;

        var foodItemPriceChanged = new FoodItemPriceChanged(itemId, oldPrice, newPrice);

        Assert.Equal(itemId, foodItemPriceChanged.ItemId);
        Assert.Equal(oldPrice, foodItemPriceChanged.OldPrice);
        Assert.Equal(newPrice, foodItemPriceChanged.NewPrice);
    }

    [Fact]
    public void ProductCreatedEvent()
    {
        var productId = Guid.NewGuid();
        var name = "name";
        var description = "description";
        var price = 40m;

        var productCreated = new ProductCreated(productId, name, description, price);

        Assert.Equal(productId, productCreated.ProductId);
        Assert.Equal(name, productCreated.Name);
        Assert.Equal(description, productCreated.Description);
        Assert.Equal(price, productCreated.Price);
    }

    [Fact]
    public void ProductEditedEvent()
    {
        var stripe_productId = "stripe_productId";
        var name = "name";
        var description = "description";
        var price = 40m;

        var productCreated = new ProductEdited(stripe_productId, name, description, price);

        Assert.Equal(stripe_productId, productCreated.Stripe_productId);
        Assert.Equal(name, productCreated.Name);
        Assert.Equal(description, productCreated.Description);
        Assert.Equal(price, productCreated.Price);
    }
}
