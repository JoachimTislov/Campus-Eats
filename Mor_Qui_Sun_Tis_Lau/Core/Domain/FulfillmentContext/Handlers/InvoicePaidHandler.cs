using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Handlers;

public class InvoicePaidHandler(IOrderingService orderingService, IFulfillmentService fulFillmentService) : INotificationHandler<InvoicePaid>
{
    private readonly IOrderingService _orderingService = orderingService ?? throw new ArgumentException(nameof(orderingService));
    private readonly IFulfillmentService _fulFillmentService = fulFillmentService ?? throw new ArgumentNullException(nameof(fulFillmentService));

    public async Task Handle(InvoicePaid notification, CancellationToken cancellationToken)
    {
        var order = await _orderingService.GetOrderById(notification.OrderId);
        if (order == null) return;

        await _fulFillmentService.UpdateOfferToRefundable(notification.OrderId, order);
    }
}