using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.Enum;
using Microsoft.AspNetCore.Mvc;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Admin.AdminOverview;

[Authorize(Roles = "Admin")]
public class AdminOverviewModel(IOrderingRepository orderingRepository, IOrderingService orderingService) : PageModel
{
    private readonly IOrderingRepository _orderingRepository = orderingRepository ?? throw new ArgumentNullException(nameof(orderingRepository));
    private readonly IOrderingService _orderingService = orderingService ?? throw new ArgumentNullException(nameof(orderingService));

    [BindProperty(SupportsGet = true)]
    public FilterTypeEnum FilterType { get; set; } = FilterTypeEnum.All;
    public List<Order> Orders { get; private set; } = [];

    public int OrdersByStatus(OrderStatusEnum status)
    {
        return Orders.Count(o => o.Status == status);
    }

    public async Task OnGetAsync()
    {

        if (FilterType == FilterTypeEnum.ThisMonth)
        {
            Orders = await _orderingService.GetMonthOrders();
        }
        else
        {
            Orders = await _orderingRepository.GetAllOrders();
        }

        Orders = Orders.OrderBy(o => o.Status).ToList();
    }
}