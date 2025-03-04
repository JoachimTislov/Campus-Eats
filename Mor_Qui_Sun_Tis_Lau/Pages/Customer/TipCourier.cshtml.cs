using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.Customer.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Customer;

[Authorize(Roles = "Customer")]
public class TipModel(IOrderingRepository orderingRepository) : PageModel
{
    private readonly IOrderingRepository _orderingRepository = orderingRepository ?? throw new ArgumentNullException(nameof(orderingRepository));


    [BindProperty(SupportsGet = true)]
    public Guid OrderId { get; set; }

    public Order? Order { get; set; }
    public OrderingViewModel DeliveryViewModel { get; set; } = new();


    public async Task<IActionResult> OnGetAsync()
    {
        Order = await _orderingRepository.GetOrderById(OrderId);
        if (Order == null) return RedirectToPage(UrlProvider.Index);

        return Page();
    }
}
