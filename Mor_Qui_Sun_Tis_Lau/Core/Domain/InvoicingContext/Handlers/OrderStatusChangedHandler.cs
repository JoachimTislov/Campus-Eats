using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Handlers;

public class OrderStatusUpdateHandler(IOrderingService orderingService, IInvoicingRepository invoiceRepository) : INotificationHandler<OrderStatusChanged>
{
    private readonly IOrderingService _orderingService = orderingService ?? throw new ArgumentNullException(nameof(orderingService));
    private readonly IInvoicingRepository _invoiceRepository = invoiceRepository ?? throw new ArgumentException(nameof(invoiceRepository));

    public async Task Handle(OrderStatusChanged notification, CancellationToken cancellationToken)
    {
        var (orderId, status, oldStatus) = notification;

        if (status == OrderStatusEnum.Canceled || status == OrderStatusEnum.Missing)
        {
            if (status == OrderStatusEnum.Canceled && oldStatus == OrderStatusEnum.Picked)
            {
                var (success, deliveryFee) = await _orderingService.GetDeliveryFeeForGivenOrderByOrderId(orderId);
                if (success)
                {
                    await _invoiceRepository.CancelOrderPaymentOfInvoiceAndReplaceWithDeliveryFee(orderId, deliveryFee);
                    return;
                }
            }

            await _invoiceRepository.CreditInvoiceByOrderId(orderId);
        }
    }
}