using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Customer;

[Authorize(Roles = "Customer")]
public class OrdersModel(IUserRepository userRepository, IOrderingService orderingService, IInvoicingRepository invoicingRepository) : PageModel
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IOrderingService _orderingService = orderingService ?? throw new ArgumentNullException(nameof(orderingService));
    private readonly IInvoicingRepository _invoicingRepository = invoicingRepository ?? throw new ArgumentNullException(nameof(invoicingRepository));

    public List<Order> UserOrders { get; private set; } = [];
    public Dictionary<Guid, InvoiceStatusEnum> OrderInvoices { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var user = await _userRepository.GetUserByClaimsPrincipal(User);
        if (user == null) return;

        UserOrders = await _orderingService.GetOrderHistoryByUserId(user.Id);
        foreach (var order in UserOrders)
        {
            var invoice = await _invoicingRepository.GetInvoiceByOrderId(order.Id);
            if (invoice != null) OrderInvoices[invoice.OrderId] = invoice.Status;
        }
    }

    public static string GetStatusMessage(OrderStatusEnum status)
    {
        return status switch
        {
            OrderStatusEnum.New => " - Order was saved, you can finish it later",
            OrderStatusEnum.Placed => " - Order is being processed by us",
            OrderStatusEnum.Picked => " - Order accepted by courier",
            OrderStatusEnum.Shipped => " - Your order is being delivered",
            OrderStatusEnum.Delivered => " - Order was successfully delivered to you",
            OrderStatusEnum.Missing => " Your order wen't missing :) ",
            OrderStatusEnum.Canceled => " - Order was canceled",
            _ => "Unknown status"
        };
    }

    public async Task<bool> CanCancelOrder(Guid orderId)
    {
        return await _orderingService.CanCancelOrder(orderId);
    }

    public async Task<IActionResult> OnPostCancelOrderAsync(Guid orderId, OrderStatusEnum orderStatus)
    {
        // Needed since everything isn't update correctly in frontend, frontend is outdated
        if (await CanCancelOrder(orderId))
        {
            await _orderingService.SetOrderStatus(orderId, OrderStatusEnum.Canceled, orderStatus);
        }

        return RedirectToPage();
    }
}