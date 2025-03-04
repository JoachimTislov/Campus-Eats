using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Handlers;

public class OrderStatusUpdateHandler(IOrderingService orderingService, IFulfillmentService fulFillmentService) : INotificationHandler<OrderStatusChanged>
{
    private readonly IOrderingService _orderingService = orderingService ?? throw new ArgumentException(nameof(orderingService));
    private readonly IFulfillmentService _fulFillmentService = fulFillmentService ?? throw new ArgumentNullException(nameof(fulFillmentService));

    public async Task Handle(OrderStatusChanged notification, CancellationToken cancellationToken)
    {
        var (orderId, status, _) = notification;

        var order = await _orderingService.GetOrderById(orderId);
        if (order == null) return;

        /*
            Creating offer for an order
        */
        if (status == OrderStatusEnum.Placed)
        {
            await _fulFillmentService.CreateOffer(orderId, order.CustomerId, order.TotalCost());
        }

        /*
            Handling case where a user either cancels the order or the order goes missing.
            The order will be refunded if its paid or canceled 
        */
        else if (status == OrderStatusEnum.Canceled || status == OrderStatusEnum.Missing)
        {
            var offer = await _fulFillmentService.GetOfferByOrderId(orderId);
            if (offer == null) return;

            if (offer.IsRefundable())
            {
                await _fulFillmentService.RefundOrderExpenseToCostumer(orderId, order);
            }
            else
            {
                await _fulFillmentService.CancelOfferSinceOrderHasBeenShipped(orderId, order);
            }
        }
    }
}