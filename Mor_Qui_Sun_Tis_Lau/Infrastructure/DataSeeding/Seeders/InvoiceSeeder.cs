
using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Pages.Customer.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Seeders;

public class InvoiceSeeder(IInvoicingRepository invoicingRepository, IOrderingService orderingService, IMediator mediator)
{
    private readonly IInvoicingRepository _invoicingRepository = invoicingRepository ?? throw new ArgumentNullException(nameof(invoicingRepository));
    private readonly IOrderingService _orderingService = orderingService ?? throw new ArgumentNullException(nameof(orderingService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task SeedData()
    {
        var orders = await _orderingService.GetOrders();
        foreach (var order in orders)
        {
            if (order.Status != OrderStatusEnum.New)
            {
                var status = order.Status;
                await _orderingService.SetOrderStatus(order.Id, OrderStatusEnum.Placed);
                await _mediator.Publish(new OrderPlaced(order.Id, new InvoiceAddress()));
                order.SetStatus(status);
            }
        }
    }

    public async Task SetInvoicesToPaid()
    {
        var orders = await _orderingService.GetOrders();
        foreach (var order in orders)
        {
            var invoice = await _invoicingRepository.GetInvoiceByOrderId(order.Id);
            if (invoice == null) continue;
            await _invoicingRepository.UpdateInvoiceStatusById(invoice.Id, InvoiceStatusEnum.Paid);
        }
    }
}