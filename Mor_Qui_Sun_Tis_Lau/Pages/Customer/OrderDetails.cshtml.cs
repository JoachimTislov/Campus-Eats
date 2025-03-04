using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Customer;

[Authorize(Roles = "Customer")]
public class OrderDetailsModel(IOrderingRepository orderingRepository, IFulfillmentService fulFillmentService, IInvoicingRepository invoicingRepository) : PageModel
{
    private readonly IOrderingRepository _orderingRepository = orderingRepository ?? throw new ArgumentNullException(nameof(orderingRepository));
    private readonly IFulfillmentService _fulFillmentService = fulFillmentService ?? throw new ArgumentNullException(nameof(fulFillmentService));
    private readonly IInvoicingRepository _invoicingRepository = invoicingRepository ?? throw new ArgumentNullException(nameof(invoicingRepository));


    [BindProperty(SupportsGet = true)]
    public Guid OrderId { get; set; }

    public Order? Order { get; set; }
    public Invoice? Invoice { get; set; }
    public Offer? Offer { get; set; }


    public async Task<IActionResult> OnGetAsync()
    {
        return await LoadOrder();
    }

    private async Task<IActionResult> LoadOrder()
    {
        Order = await _orderingRepository.GetOrderById(OrderId);
        if (Order == null) return RedirectToPage(UrlProvider.Index);

        if (!Order.IsNew())
        {
            Invoice = await _invoicingRepository.GetInvoiceByOrderId(OrderId);
            Offer = await _fulFillmentService.GetOfferByOrderId(OrderId);
        }

        return Page();
    }
}
