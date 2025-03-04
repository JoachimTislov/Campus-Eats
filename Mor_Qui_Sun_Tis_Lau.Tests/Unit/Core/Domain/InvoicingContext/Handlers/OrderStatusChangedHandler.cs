
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.InvoicingContext.Handlers;

public class OrderStatusUpdateHandlerTests
{
    private readonly Mock<IOrderingService> _mockOrderingService = new();
    private readonly Mock<IInvoicingRepository> _mockInvoicingRepository = new();

    private readonly OrderStatusUpdateHandler _orderStatusUpdateHandler;

    public OrderStatusUpdateHandlerTests()
    {
        _orderStatusUpdateHandler = new(_mockOrderingService.Object, _mockInvoicingRepository.Object);
    }

    [Fact]
    public async Task OrderStatusUpdateHandler_ShouldCreditInvoice_WhenOrderStatusIsCanceled()
    {
        var orderId = Guid.NewGuid();
        var orderStatusChanged = new OrderStatusChanged(orderId, OrderStatusEnum.Canceled);

        await _orderStatusUpdateHandler.Handle(orderStatusChanged, CancellationToken.None);

        _mockInvoicingRepository.Verify(m => m.CreditInvoiceByOrderId(orderId), Times.Once);
    }

    [Fact]
    public async Task OrderStatusUpdateHandler_ShouldNotCreditInvoice_WhenOrderStatusIsNotCanceled()
    {
        var orderId = Guid.NewGuid();
        var orderStatusChanged = new OrderStatusChanged(orderId, OrderStatusEnum.Placed);

        await _orderStatusUpdateHandler.Handle(orderStatusChanged, CancellationToken.None);

        _mockInvoicingRepository.Verify(m => m.CreditInvoiceByOrderId(orderId), Times.Never);
    }
}