using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Microsoft.AspNetCore.Mvc;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Pages.Courier.Enums;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Courier;

[Authorize(Roles = "Courier")]
public class CourierOrderOverviewModel(IUserRepository userRepository, IOrderingRepository orderingRepository, IOrderingService orderingService) : PageModel
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IOrderingRepository _orderingRepository = orderingRepository ?? throw new ArgumentNullException(nameof(orderingRepository));
    private readonly IOrderingService _orderingService = orderingService ?? throw new ArgumentNullException(nameof(orderingService));

    public User? CourierUser { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid OrderId { get; set; }


    public Order? Order { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!await GetOrderOverviewInformation()) return RedirectToPage(UrlProvider.Courier.CourierDashboard);

        return Page();
    }

    public async Task<IActionResult> OnPostChangeStatusAsync(OrderStatusEnum OrderStatusEnum)
    {
        if (!await GetOrderOverviewInformation()) return RedirectToPage(UrlProvider.Courier.CourierDashboard);

        await _orderingService.SetOrderStatus(Order!.Id, OrderStatusEnum);

        return Page();
    }

    private async Task<bool> GetOrderOverviewInformation()
    {
        CourierUser = await _userRepository.GetUserByHttpContext(HttpContext);
        Order = await _orderingRepository.GetOrderById(OrderId);
        if (Order == null || CourierUser == null || CourierUser.Id != Order.CourierId) return false;
        return true;
    }

}