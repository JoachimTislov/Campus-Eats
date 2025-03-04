using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.OrderingContext;

public class EventsTests
{
    [Fact]
    public void OrderStatusChanged()
    {
        var orderId = Guid.NewGuid();
        var orderStatus = OrderStatusEnum.Canceled;

        var orderStatusChanged = new OrderStatusChanged(orderId, orderStatus);

        Assert.Equal(orderId, orderStatusChanged.OrderId);
        Assert.Equal(orderStatus, orderStatusChanged.Status);
    }
}