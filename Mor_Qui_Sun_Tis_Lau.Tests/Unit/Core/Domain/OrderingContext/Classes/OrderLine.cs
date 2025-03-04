using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.OrderingContext.Classes;

public class OrderLineTests
{
    [Fact]
    public void ConstructorWithValues_ShouldAssignValues()
    {
        // Arrange
        var name = "Hotdog";
        var price = 10m;
        var quantity = 1;
        var id = Guid.NewGuid();
        var stripeId = "stripeId";


        // Act
        var orderLine = new OrderLine(name, price, quantity, id, stripeId);

        // Assert
        Assert.Equal(Guid.Empty, orderLine.Id);
        Assert.Equal(name, orderLine.Name);
        Assert.Equal(price, orderLine.Price);
        Assert.Equal(quantity, orderLine.Quantity);
        Assert.Equal(id, orderLine.OrderId);
        Assert.Equal(stripeId, orderLine.Stripe_productId);
    }

    [Fact]
    public void AssertOrderLineSetters()
    {
        var type = typeof(OrderLine);

        AssertSetter.AssertProtected(type, nameof(OrderLine.Id));
        AssertSetter.AssertPrivate(type, nameof(OrderLine.OrderId));
        AssertSetter.AssertProtected(type, nameof(OrderLine.Name));
        AssertSetter.AssertProtected(type, nameof(OrderLine.Price));
        AssertSetter.AssertProtected(type, nameof(OrderLine.Quantity));
    }
}