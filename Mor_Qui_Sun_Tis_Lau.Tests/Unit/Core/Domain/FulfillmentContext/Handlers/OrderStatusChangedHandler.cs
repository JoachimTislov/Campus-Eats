using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.FulfillmentContext.Handlers;

public class OrderStatusUpdateHandlerTests
{
    private readonly Mock<IOrderingService> _mockOrderingService = new();
    private readonly Mock<IFulfillmentService> _mockFulFillmentService = new();

    private readonly OrderStatusUpdateHandler _orderStatusUpdateHandler;

    public OrderStatusUpdateHandlerTests()
    {
        _orderStatusUpdateHandler = new(_mockOrderingService.Object, _mockFulFillmentService.Object);
    }

    [Fact]
    public async Task OrderStatusUpdateHandler_ShouldReturn_WhenOrderStatusIsNotPlacedOrDeclined()
    {
        var orderId = Guid.NewGuid();
        var orderStatusChanged = new OrderStatusChanged(orderId, OrderStatusEnum.Canceled);

        await _orderStatusUpdateHandler.Handle(orderStatusChanged, CancellationToken.None);

        _mockOrderingService.Verify(m => m.GetOrderById(orderId), Times.Once);
        _mockFulFillmentService.Verify(m => m.CreateOffer(orderId, It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task OrderStatusUpdateHandler_ShouldReturn_WhenOrderIsNull()
    {
        var orderId = Guid.NewGuid();

        Order? order = null;
        _mockOrderingService
            .Setup(m => m.GetOrderById(orderId))
            .ReturnsAsync(order);

        var orderStatusChanged = new OrderStatusChanged(orderId, OrderStatusEnum.Placed);

        await _orderStatusUpdateHandler.Handle(orderStatusChanged, CancellationToken.None);

        _mockOrderingService.Verify(m => m.GetOrderById(orderId), Times.Once);

        _mockFulFillmentService.Verify(m => m.CreateOffer(orderId, It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task OrderStatusUpdateHandler_ShouldCreateOrder_WhenOrderIsPlaced()
    {
        var orderId = Guid.NewGuid();

        var order = new Order();
        _mockOrderingService
            .Setup(m => m.GetOrderById(orderId))
            .ReturnsAsync(order);

        var orderStatusChanged = new OrderStatusChanged(orderId, OrderStatusEnum.Placed);

        await _orderStatusUpdateHandler.Handle(orderStatusChanged, CancellationToken.None);

        _mockOrderingService.Verify(m => m.GetOrderById(orderId), Times.Once);
        _mockFulFillmentService.Verify(m => m.CreateOffer(orderId, It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Once);
    }

    [Fact]
    public async Task OrderStatusUpdateHandler_ShouldRefundOrderExpense_WhenOrderIsDeclined()
    {
        var orderId = Guid.NewGuid();

        var order = new Order();
        _mockOrderingService
            .Setup(m => m.GetOrderById(orderId))
            .ReturnsAsync(order);

        var orderStatusChanged = new OrderStatusChanged(orderId, OrderStatusEnum.Canceled);

        await _orderStatusUpdateHandler.Handle(orderStatusChanged, CancellationToken.None);

        _mockOrderingService.Verify(m => m.GetOrderById(orderId), Times.Once);
        _mockFulFillmentService.Verify(m => m.CreateOffer(orderId, It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
    }
}