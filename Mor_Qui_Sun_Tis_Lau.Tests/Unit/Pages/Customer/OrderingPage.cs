using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.Session;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.Customer;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Customer;

public class OrderingPageTests
{
    private readonly Mock<IMediator> _mockMediator = new();
    private readonly Mock<IFulfillmentService> _mockFulfillmentService = new();
    private readonly Mock<IInvoicingRepository> _mockInvoicingRepository = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IOrderingService> _mockOrderingService = new();
    private readonly Mock<IStripeSessionService> _mockStripeSessionService = new();

    private readonly OrderingPage _orderingPageModel;

    public OrderingPageTests()
    {
        _orderingPageModel = new(_mockMediator.Object, _mockFulfillmentService.Object, _mockInvoicingRepository.Object, _mockUserRepository.Object, _mockOrderingService.Object, _mockStripeSessionService.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        ], "mock"));

        var httpContext = new DefaultHttpContext
        {
            User = user
        };

        _orderingPageModel.PageContext = new PageContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task OnPostPlaceOrderAsync_ReturnsToCanteenWhenOrderIsNull()
    {
        var customer = new User("firstName", "lastName", "email@example.com");
        var order = new Order(customer.Id);

        _mockOrderingService
            .Setup(o => o.GetOrderById(It.IsAny<Guid>()))
            .ReturnsAsync((Order?)null);

        // Act
        _orderingPageModel.OrderId = order.Id;
        var result = await _orderingPageModel.OnPostPlaceOrderAsync();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(UrlProvider.Customer.Orders, redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostPlaceOrderAsync_ReturnsToCanteenWhenShopUserIsNull()
    {

        var customer = new User("firstName", "lastName", "email@example.com");
        var order = new Order(customer.Id);

        _mockOrderingService
            .Setup(o => o.GetOrderById(It.IsAny<Guid>()))
            .ReturnsAsync(order);

        // Act
        _orderingPageModel.OrderId = order.Id;
        var result = await _orderingPageModel.OnPostPlaceOrderAsync();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(UrlProvider.Customer.Orders, redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostAsync_ReturnsToCanteenWhenOrderIdIsNull()
    {

        var customer = new User("firstName", "lastName", "email@example.com");
        var order = new Order(customer.Id);

        _mockOrderingService
            .Setup(o => o.GetOrderById(It.IsAny<Guid>()))
            .ReturnsAsync(order);

        // Act
        _orderingPageModel.OrderId = Guid.Empty;
        var result = await _orderingPageModel.OnPostPlaceOrderAsync();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(UrlProvider.Customer.Orders, redirectResult.PageName);
    }
}