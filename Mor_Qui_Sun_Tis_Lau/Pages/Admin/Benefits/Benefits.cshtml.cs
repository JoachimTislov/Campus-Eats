using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.Enum;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Admin.Benefits;

[Authorize(Roles = "Admin")]
public class BenefitsModel(IOrderingRepository orderingRepository, IOrderingService orderingService, IInvoicingRepository invoicingRepository) : PageModel
{
    private readonly IOrderingRepository _orderingRepository = orderingRepository ?? throw new ArgumentNullException(nameof(orderingRepository));
    private readonly IOrderingService _orderingService = orderingService ?? throw new ArgumentNullException(nameof(orderingService));
    private readonly IInvoicingRepository _invoicingRepository = invoicingRepository?? throw new ArgumentNullException(nameof(invoicingRepository));

    public decimal totalEarned = 0m;
    public decimal totalEarnedServiceFee = 0m;
    [BindProperty(SupportsGet = true)]
    public FilterTypeEnum FilterType { get; set; } = FilterTypeEnum.All;
    public Dictionary<Order, Invoice> Invoices { get; set; } = [];
    public List<Order> Orders { get; set; } = [];
    public async Task OnGetAsync()
    {
        await GetBenefitsInformation();
    }

    private async Task GetBenefitsInformation()
    {
        if (FilterType == FilterTypeEnum.ThisMonth)
        {
            Orders = await _orderingService.GetMonthOrders();
        }
        else
        {
            Orders = await _orderingRepository.GetAllOrders();
        }
        
        foreach (Order order in Orders)
        {
            var invoice = await _invoicingRepository.GetInvoiceByOrderId(order.Id);
            if (invoice != null) Invoices[order] = invoice;
        }
        CalculateTotalEarned();
    }

    private void CalculateTotalEarned()
    {
        totalEarned = Invoices.Where(i => i.Value.Status == InvoiceStatusEnum.Paid).Select(i => i.Value.PaymentDue).Sum();
        totalEarnedServiceFee = Invoices.Where(i => i.Value.Status == InvoiceStatusEnum.Paid).Select(o => o.Key.AdminDeliveryFeeCut).Sum();
    }
}