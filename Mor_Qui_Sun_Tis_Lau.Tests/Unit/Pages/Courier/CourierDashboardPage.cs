using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Pages.Courier;
using Mor_Qui_Sun_Tis_Lau.Pages.Courier.Enums;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Courier;
public class CourierDashboardPageTests
{
    private readonly Mock<IOrderingService> _mockOrderingService = new();
    private readonly Mock<IOrderingRepository> _mockOrderingRepository = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IInvoicingRepository> _mockInvoicingRepository  = new();
    private readonly CourierDashBoardModel _courierDashboardModel;

    private List<Order>? TestOrdersPlaced { get; set; }
    private List<Order>? TestOrdersHistory { get; set; }
    private Dictionary<Order, Invoice>? TestOrdersInvoices { get; set; } = [];
    private User? TestUser { get; set; }
    private readonly DateTime? Date = null;

    public CourierDashboardPageTests()
    {
        _courierDashboardModel = new(_mockUserRepository.Object, _mockOrderingRepository.Object, _mockOrderingService.Object, _mockInvoicingRepository.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        ], "mock"));

        var httpContext = new DefaultHttpContext
        {
            User = user
        };

        _courierDashboardModel.PageContext = new PageContext
        {
            HttpContext = httpContext
        };
    }

    private void MockGetDashboardInformation(int year, int month)
    {
        TestUser = new("Test", "User", "testuser@example.com");
        DateTime? date = null;
        if (year != 0) date = new(year, month, 1);

        TestOrdersPlaced ??= [];
        for (var i = 0; i < 4; i++)
        {
            var order = new Order();
            order.SetStatus(OrderStatusEnum.Placed);
            TestOrdersPlaced.Add(order);
        }

        var orderValues = new[]
        {
            new { Status = OrderStatusEnum.Delivered, Date = new DateTime(2024, 9, 1) },
            new { Status = OrderStatusEnum.Delivered, Date = new DateTime(2024, 8, 1) },
            new { Status = OrderStatusEnum.Delivered, Date = new DateTime(2024, 9, 1) },
            new { Status = OrderStatusEnum.Delivered, Date = new DateTime(2024, 9, 1) },
            new { Status = OrderStatusEnum.Shipped, Date = new DateTime(2024, 9, 1) },
            new { Status = OrderStatusEnum.Missing, Date = new DateTime(2024, 9, 1) }
        };

        TestOrdersHistory ??= [];
        TestOrdersInvoices ??= [];
        foreach (var values in orderValues)
        {
            var orderId = Guid.NewGuid();
            var order = new Order() { OrderDate = values.Date };
            var customer = new User();
            order.SetStatus(values.Status);
            TestOrdersHistory.Add(order);
            
            InvoiceAddress invoiceAddresss = new();
            var invoice = new Invoice(order.Id, customer, order.TotalCost(), invoiceAddresss);

            TestOrdersInvoices[order] = invoice;
        }
        // Set tips
        TestOrdersHistory[0].SetTip(5m);
        TestOrdersHistory[1].SetTip(5m);
        TestOrdersHistory[5].SetTip(5m);

        TestOrdersHistory = [.. TestOrdersHistory.OrderBy(o => o.Status)];

        _mockUserRepository
            .Setup(u => u.GetUserByHttpContext(It.IsAny<HttpContext>()))
            .ReturnsAsync(TestUser);

        _mockOrderingRepository
            .Setup(o => o.GetOrdersByOrderStatus(It.IsAny<OrderStatusEnum>()))
            .ReturnsAsync(TestOrdersPlaced);

        _mockOrderingRepository
            .Setup(o => o.GetOrdersByCourierId(It.IsAny<Guid>()))
            .ReturnsAsync(TestOrdersHistory);

        foreach (Order order in TestOrdersHistory)
        {
            _mockInvoicingRepository
                .Setup(i => i.GetInvoiceByOrderId(It.Is<Guid>(o => o == order.Id)))
                .ReturnsAsync(TestOrdersInvoices[order]);
        }
    }

    [Theory]
    [InlineData(CourierDashboardPageEnum.Earnings, 2024, 9, 0, 0, 0)]
    [InlineData(CourierDashboardPageEnum.Earnings, 2024, 5, 0, 0, 0)]
    [InlineData(CourierDashboardPageEnum.OrderOverview, 0, 0, 15, 0, 15)]
    public async Task OnGetAsync_GetDashboardInformation(CourierDashboardPageEnum page, int year, int month, decimal totalEarning, decimal totalDeliveryEarning, decimal totalTips)
    {
        MockGetDashboardInformation(year, month);

        _courierDashboardModel.Invoices = TestOrdersInvoices ?? [];
        await _courierDashboardModel.OnGetAsync(page, year, month);

        if (Date != null && _courierDashboardModel.ActivePage != CourierDashboardPageEnum.Earnings)
        {
            Assert.NotNull(_courierDashboardModel.SelectedEarningsMonth);
            Assert.NotEqual(Date.Value.Month, _courierDashboardModel.SelectedEarningsMonth.Value.Month);
            Assert.NotEqual(Date.Value.Year, _courierDashboardModel.SelectedEarningsMonth.Value.Year);
        }
        else
        {
            Assert.NotNull(_courierDashboardModel.AllEarningsMonths);
        }

        Assert.Equal(TestUser, _courierDashboardModel.Courier);
        Assert.Equal(TestOrdersPlaced, _courierDashboardModel.OrdersAvailableForPickup);
        Assert.Equal(TestOrdersHistory, _courierDashboardModel.CourierOrderHistory);
        Assert.Equal(TestOrdersInvoices, _courierDashboardModel.Invoices);
        Assert.Equal(totalEarning, _courierDashboardModel.EarningsToShow[EarningsEnum.Total]);
        Assert.Equal(totalDeliveryEarning, _courierDashboardModel.EarningsToShow[EarningsEnum.DeliveryFeeCut]);
        Assert.Equal(totalTips, _courierDashboardModel.EarningsToShow[EarningsEnum.Tips]);
        _mockUserRepository.Verify(o => o.GetUserByHttpContext(It.IsAny<HttpContext>()), Times.Once());
        _mockOrderingRepository.Verify(o => o.GetOrdersByOrderStatus(It.Is<OrderStatusEnum>(s => s == OrderStatusEnum.Placed)), Times.Once());
        _mockOrderingRepository.Verify(o => o.GetOrdersByCourierId(It.Is<Guid>(c => c == TestUser!.Id)), Times.Once());

    }

    /*[Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, true, true)]
    public async Task OnPostTakeOrShowOrderAsync_Tests(bool isUserNull, bool isOrderNull, bool isSuccessFalse)
    {
        TestUser = new("Test", "User", "testuser@example.com");
        Order testOrder = new(Guid.NewGuid());

        TestOrdersPlaced ??= [];
        for (var i = 0; i < 2; i++)
        {
            var order = new Order();
            order.SetStatus(OrderStatusEnum.Placed);
            TestOrdersPlaced.Add(order);
        }

        // Total courier payout 26
        // Total order delivery fee 16
        TestOrdersHistory =
        [
            new Order(TestUser.Id) { OrderDate = new DateTime(2024, 9, 1) },
            new Order(TestUser.Id) { OrderDate = new DateTime(2024, 8, 1) },
        ];
        // Total order tips 10
        TestOrdersHistory[0].SetTip(5m);
        TestOrdersHistory[0].SetStatus(OrderStatusEnum.Delivered);
        TestOrdersHistory[1].SetTip(5m);
        TestOrdersHistory[1].SetStatus(OrderStatusEnum.Delivered);

        TestOrdersHistory = [.. TestOrdersHistory.OrderBy(o => o.Status)];

        if (!isUserNull)
        {
            _mockUserRepository
                .Setup(u => u.GetUserByHttpContext(It.IsAny<HttpContext>()))
                .ReturnsAsync(TestUser);
        }

        if (!isOrderNull)
        {
            _mockOrderingRepository
                .Setup(o => o.GetOrderById(It.IsAny<Guid>()))
                .ReturnsAsync(testOrder);
        }

        _mockOrderingRepository
            .Setup(o => o.GetOrdersByOrderStatus(It.IsAny<OrderStatusEnum>()))
            .ReturnsAsync(TestOrdersPlaced);

        _mockOrderingRepository
            .Setup(o => o.GetOrdersByCourierId(It.IsAny<Guid>()))
            .ReturnsAsync(TestOrdersHistory);

        var result = await _courierDashboardModel.OnPostTakeOrShowOrderAsync(testOrder.Id, true, CourierDashboardPageEnum.OrderOverview);
        if (!isSuccessFalse) testOrder.SetStatus(OrderStatusEnum.Shipped);

        if (!isUserNull && !isOrderNull && !isSuccessFalse)
        {
            Assert.Equal(OrderStatusEnum.Shipped, testOrder.Status);
            Assert.Equal(TestUser.Id, testOrder.CourierId);
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(UrlProvider.Courier.CourierOrderOverview, redirectResult.PageName);
            Assert.NotNull(redirectResult.RouteValues);
            Assert.Equal(testOrder.Id, redirectResult.RouteValues["orderId"]);
        }
        else if (isUserNull)
        {
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(UrlProvider.Index, redirectResult.PageName);
        }
        else if (isOrderNull)
        {
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(UrlProvider.Courier.CourierDashboard, redirectResult.PageName);
        }
        else if (isSuccessFalse)
        {
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(UrlProvider.Courier.CourierDashboard, redirectResult.PageName);
        }
    }*/

}