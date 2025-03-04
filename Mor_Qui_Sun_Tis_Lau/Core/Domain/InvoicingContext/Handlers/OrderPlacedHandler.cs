using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Handlers;

public class OrderPlacedHandler(IUserRepository userRepository, IOrderingService orderingService, IInvoicingRepository invoiceRepository) : INotificationHandler<OrderPlaced>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentException(nameof(userRepository));
    private readonly IOrderingService _orderingService = orderingService ?? throw new ArgumentException(nameof(orderingService));
    private readonly IInvoicingRepository _invoiceRepository = invoiceRepository ?? throw new ArgumentException(nameof(invoiceRepository));

    public async Task Handle(OrderPlaced notification, CancellationToken cancellationToken)
    {
        var (orderId, invoiceAddress) = notification;

        var order = await _orderingService.GetOrderById(orderId);
        if (order == null) return;

        var customer = await _userRepository.GetUserById(order.CustomerId);
        if (customer == null) return;

        await _invoiceRepository.CreateInvoice(orderId, customer, order.TotalCost(), invoiceAddress);
    }
}