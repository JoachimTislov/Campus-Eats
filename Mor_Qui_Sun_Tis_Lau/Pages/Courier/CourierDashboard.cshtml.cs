using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Mor_Qui_Sun_Tis_Lau.Pages.Courier.Enums;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Courier;

[Authorize(Roles = "Courier")]
public class CourierDashBoardModel(IUserRepository userRepository, IOrderingRepository orderingRepository, IOrderingService orderingService, IInvoicingRepository invoicingRepository) : PageModel
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IOrderingRepository _orderingRepository = orderingRepository ?? throw new ArgumentNullException(nameof(orderingRepository));
    private readonly IOrderingService _orderingService = orderingService ?? throw new ArgumentNullException(nameof(orderingService));
    private readonly IInvoicingRepository _invoicingRepository = invoicingRepository?? throw new ArgumentNullException(nameof(invoicingRepository));
    public CourierDashboardPageEnum ActivePage = CourierDashboardPageEnum.OrderOverview;

    public User? Courier;
    public List<Order>? OrdersAvailableForPickup;
    public List<Order>? CourierOrderHistory;
    public Dictionary<Order, Invoice> Invoices = [];
    public decimal TotalTips = 0m;
    public decimal TotalDeliveryEarnings = 0m;
    public decimal TotalEarnings => TotalDeliveryEarnings + TotalTips;
    public DateTime? SelectedEarningsMonth { get; set; }
    public Dictionary<int, List<string>>? AllEarningsMonths { get; set; }
    public Dictionary<string, List<decimal>>? AllEarningsThisMonth { get; set; }
    public Dictionary<EarningsEnum, decimal> EarningsToShow = new() { { EarningsEnum.Total, 0m }, { EarningsEnum.DeliveryFeeCut, 0m }, { EarningsEnum.Tips, 0m } };

    public async Task OnGetAsync(CourierDashboardPageEnum? activePage, int? selectedYear, int? selectedMonth)
    {
        await GetDashboardInformation(activePage, selectedYear, selectedMonth);
    }

    public async Task<IActionResult> OnPostTakeOrShowOrderAsync(Guid orderId, bool takingOrder, CourierDashboardPageEnum? activePage)
    {
        await GetDashboardInformation(activePage);

        var order = await _orderingRepository.GetOrderById(orderId);
        if (Courier == null) return RedirectToPage(UrlProvider.Index);
        if (order == null) return RedirectToPage(UrlProvider.Courier.CourierDashboard);

        if (takingOrder)
        {
            await _orderingService.PickOrder(order.Id, Courier.Id);
        }

        return RedirectToPage(UrlProvider.Courier.CourierOrderOverview, new { orderId = order.Id });
    }

    private async Task GetDashboardInformation(CourierDashboardPageEnum? activePage, int? selectedYear = null, int? selectedMonth = null)
    {
        ActivePage = activePage ?? CourierDashboardPageEnum.OrderOverview;

        Courier = await _userRepository.GetUserByHttpContext(HttpContext);
        if (Courier == null) return;
        OrdersAvailableForPickup = await _orderingRepository.GetOrdersByOrderStatus(OrderStatusEnum.Placed);
        CourierOrderHistory = await _orderingRepository.GetOrdersByCourierId(Courier.Id);

        await GetInvoicesByOrders();

        var getEarningsByMonthSuccess = false;
        if (CourierOrderHistory.Count > 0) GetAllMonthsSinceFirstOrderCompleted();
        if (selectedYear != null && selectedMonth != null && selectedYear > 0 && selectedMonth > 0)
            getEarningsByMonthSuccess = GetEarningsByMonth(selectedYear.Value, selectedMonth.Value);

        CourierOrderHistory = [.. CourierOrderHistory.OrderBy(o => o.Status)];

        TotalTips = CourierOrderHistory.Sum(o => o.Tip);
        TotalDeliveryEarnings = Invoices.Where(i => i.Value.Status == InvoiceStatusEnum.Paid).Select(o => o.Key.CourierDeliveryFeeCut).Sum();

        if (!getEarningsByMonthSuccess)
        {
            EarningsToShow[EarningsEnum.Total] = TotalEarnings;
            EarningsToShow[EarningsEnum.DeliveryFeeCut] = TotalDeliveryEarnings;
            EarningsToShow[EarningsEnum.Tips] = TotalTips;
        }
    }

    private async Task GetInvoicesByOrders()
    {
        if (CourierOrderHistory == null) return;
        foreach (var order in CourierOrderHistory)
        {
            var invoice = await _invoicingRepository.GetInvoiceByOrderId(order.Id);
            if (invoice != null) Invoices[order] = invoice;
        }
    }

    private void GetAllMonthsSinceFirstOrderCompleted()
    {
        DateTime earliest = DateTime.MinValue;
        DateTime latest = DateTime.MaxValue;

        if (CourierOrderHistory!.Count > 0)
        {
            earliest = new DateTime(CourierOrderHistory.Min(d => d.OrderDate.Year), CourierOrderHistory.Min(d => d.OrderDate.Month), 1);
            latest = new DateTime(CourierOrderHistory.Max(d => d.OrderDate.Year), CourierOrderHistory.Max(d => d.OrderDate.Month), 1);
        }

        Dictionary<int, List<string>> yearMonthDict = [];
        DateTime current = earliest;
        while (current <= latest)
        {
            int year = current.Year;
            string monthName = current.ToString("MMMM", CultureInfo.InvariantCulture);

            if (!yearMonthDict.ContainsKey(year))
            {
                yearMonthDict[year] = [];
            }

            if (!yearMonthDict[year].Contains(monthName))
            {
                yearMonthDict[year].Add(monthName);
            }

            current = current.AddMonths(1);
        }

        AllEarningsMonths = yearMonthDict;

    }

    private bool GetEarningsByMonth(int selectedYear, int selectedMonth)
    {
        SelectedEarningsMonth = new DateTime(selectedYear, selectedMonth, 1);

        if (SelectedEarningsMonth.HasValue)
        {
            var filteredOrders = Invoices
                .Where(o => o.Key.OrderDate.Month == SelectedEarningsMonth.Value.Month && o.Key.OrderDate.Year == SelectedEarningsMonth.Value.Year && o.Value.Status == InvoiceStatusEnum.Paid)
                .GroupBy(o => o.Key.OrderDate.ToString("MMM"))
                .ToDictionary(g => g.Key, g => new List<decimal> { g.Sum(o => o.Key.CourierEarning), g.Sum(o => o.Key.CourierDeliveryFeeCut), g.Sum(o => o.Key.Tip) });
            if (filteredOrders != null && filteredOrders.FirstOrDefault().Value != null)
            {
                EarningsToShow[EarningsEnum.Total] = filteredOrders.FirstOrDefault().Value[0];
                EarningsToShow[EarningsEnum.DeliveryFeeCut] = filteredOrders.FirstOrDefault().Value[1];
                EarningsToShow[EarningsEnum.Tips] = filteredOrders.FirstOrDefault().Value[2];
            }
            else if (selectedYear > 0 && selectedMonth > 0)
            {
                EarningsToShow[EarningsEnum.Total] = 0;
                EarningsToShow[EarningsEnum.DeliveryFeeCut] = 0;
                EarningsToShow[EarningsEnum.Tips] = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;

    }
}