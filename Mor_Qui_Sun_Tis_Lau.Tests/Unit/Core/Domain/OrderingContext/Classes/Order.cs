using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.OrderingContext.Classes;

public class OrderTest
{
    [Fact]
    public void ConstructorWithValues_ShouldAssignValuesAndDefaultValues()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        // Act
        var order = new Order(customerId);

        // Assert
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(0m, order.Tip);
        Assert.Equal(50m, order.DeliveryFee);
        Assert.Equal(OrderStatusEnum.New, order.Status);
    }

    [Theory]
    [InlineData(100.0, true)]
    [InlineData(0.0, true)]
    [InlineData(-2.0, false)]
    public void Function_SetDeliveryFeeOrTip(decimal valueChange, bool shouldUpdateDeliveryFee)
    {
        var customerId = Guid.NewGuid();
        var order = new Order(customerId);

        order.SetDeliveryFee(valueChange);
        order.SetTip(valueChange);

        if (shouldUpdateDeliveryFee)
        {
            Assert.Equal(valueChange, order.DeliveryFee);
            Assert.Equal(valueChange, order.Tip);
        }
        else
        {
            Assert.NotEqual(valueChange, order.DeliveryFee);
            Assert.NotEqual(valueChange, order.Tip);
        }
    }

    [Fact]
    public void AddOrderLine_ShouldAddNewOrderLine()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());
        var orderName = "Hotdog";
        var orderPrice = 10m;
        var orderAmount = 1;
        var orderStripeId = "stripeId";

        // Act
        order.AddOrderLine(orderName, orderPrice, orderAmount, orderStripeId);

        // Assert
        Assert.Single(order.OrderLines);
        var orderLine = order.OrderLines.First();
        Assert.Equal(orderName, orderLine.Name);
        Assert.Equal(orderPrice, orderLine.Price);
        Assert.Equal(orderAmount, orderLine.Quantity);
        Assert.Equal(orderStripeId, orderLine.Stripe_productId);
    }

    [Fact]
    public void SetCourier_ShouldSet()
    {
        Order order = new(Guid.NewGuid());
        User courier = new("test", "test", "test@gmail.com");

        order.SetCourier(courier.Id);

        Assert.Equal(courier.Id, order.CourierId);
    }

    [Fact]
    public void CourierDeliveryFeeCutAndCourierEarning_ReturnsCorrectValues()
    {
        Order order = new(Guid.NewGuid());
        order.SetTip(5m);

        var deliveryFeeCut = order.CourierDeliveryFeeCut;
        var courierEarning = order.CourierEarning;
        var actualDeliveryFeeCut = order.DeliveryFee * 0.8m;
        var actualDeliveryEarning = (order.DeliveryFee * 0.8m) + order.Tip;

        Assert.Equal(actualDeliveryFeeCut, deliveryFeeCut);
        Assert.Equal(actualDeliveryEarning, courierEarning);
    }

    [Fact]
    public void Properties_AssertSetters()
    {
        var type = typeof(Order);

        AssertSetter.AssertProtected(type, nameof(Order.Id));
        AssertSetter.AssertPublic(type, nameof(Order.OrderDate));
        AssertSetter.AssertPublic(type, nameof(Order.CampusLocation));
        AssertSetter.AssertPrivate(type, nameof(Order.CustomerId));
        AssertSetter.AssertPrivate(type, nameof(Order.Tip));
        AssertSetter.AssertPrivate(type, nameof(Order.DeliveryFee));
        AssertSetter.AssertPrivate(type, nameof(Order.Status));
    }
}