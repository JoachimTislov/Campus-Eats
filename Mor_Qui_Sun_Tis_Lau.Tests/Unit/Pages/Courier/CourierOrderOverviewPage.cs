using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.Courier;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Courier;

public class CourierOrderOverviewPageTests
{
    private readonly Mock<IOrderingService> _mockOrderingService = new();
    private readonly Mock<IOrderingRepository> _mockOrderingRepository = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly CourierOrderOverviewModel _courierOrderOverviewModel;

    public CourierOrderOverviewPageTests()
    {
        _courierOrderOverviewModel = new(_mockUserRepository.Object, _mockOrderingRepository.Object, _mockOrderingService.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        ], "mock"));

        var httpContext = new DefaultHttpContext
        {
            User = user
        };

        _courierOrderOverviewModel.PageContext = new PageContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task OnGetAsync_ReturnsOrderOverview()
    {
        User testUser = new("Test", "User", "testuser@example.com");
        Order testOrder = new(testUser.Id);
        testOrder.SetCourier(testUser.Id);

        _mockUserRepository
            .Setup(u => u.GetUserByHttpContext(It.IsAny<HttpContext>()))
            .ReturnsAsync(testUser);

        _mockOrderingRepository
            .Setup(o => o.GetOrderById(It.IsAny<Guid>()))
            .ReturnsAsync(testOrder);

        var result = await _courierOrderOverviewModel.OnGetAsync();

        Assert.Equal(testUser, _courierOrderOverviewModel.CourierUser);
        Assert.Equal(testOrder, _courierOrderOverviewModel.Order);
        Assert.NotNull(_courierOrderOverviewModel.Order);
        Assert.NotNull(_courierOrderOverviewModel.CourierUser);
        Assert.Equal(testUser.Id, _courierOrderOverviewModel.Order.CourierId);
        _mockUserRepository.Verify(u => u.GetUserByHttpContext(It.IsAny<HttpContext>()), Times.Once);
        _mockOrderingRepository.Verify(o => o.GetOrderById(It.IsAny<Guid>()), Times.Once);
        var pageResult = Assert.IsType<PageResult>(result);
        Assert.NotNull(pageResult);

    }

    [Fact]
    public async Task OnGetAsync_ReturnsCourierDashboard_UserIsNull()
    {
        User testUser = new("Test", "User", "testuser@example.com");
        Order testOrder = new(testUser.Id);
        testOrder.SetCourier(testUser.Id);

        _mockUserRepository
            .Setup(u => u.GetUserByHttpContext(It.IsAny<HttpContext>()))
            .ReturnsAsync((User?)null);

        _mockOrderingRepository
            .Setup(o => o.GetOrderById(It.IsAny<Guid>()))
            .ReturnsAsync(testOrder);

        var result = await _courierOrderOverviewModel.OnGetAsync();

        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(UrlProvider.Courier.CourierDashboard, redirectResult.PageName);
        Assert.Null(_courierOrderOverviewModel.CourierUser);
        Assert.NotNull(_courierOrderOverviewModel.Order);
        _mockUserRepository.Verify(u => u.GetUserByHttpContext(It.IsAny<HttpContext>()), Times.Once);
        _mockOrderingRepository.Verify(o => o.GetOrderById(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_ReturnsCourierDashboard_OrderIsNull()
    {
        User testUser = new("Test", "User", "testuser@example.com");
        Order testOrder = new(testUser.Id);
        testOrder.SetCourier(testUser.Id);

        _mockUserRepository
            .Setup(u => u.GetUserByHttpContext(It.IsAny<HttpContext>()))
            .ReturnsAsync(testUser);

        _mockOrderingRepository
            .Setup(o => o.GetOrderById(It.IsAny<Guid>()))
            .ReturnsAsync((Order?)null);

        var result = await _courierOrderOverviewModel.OnGetAsync();

        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(UrlProvider.Courier.CourierDashboard, redirectResult.PageName);
        Assert.Equal(testUser, _courierOrderOverviewModel.CourierUser);
        Assert.Null(_courierOrderOverviewModel.Order);
        _mockUserRepository.Verify(u => u.GetUserByHttpContext(It.IsAny<HttpContext>()), Times.Once);
        _mockOrderingRepository.Verify(o => o.GetOrderById(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_ReturnsCourierDashboard_CourierIdDoesNotMatchUserId()
    {
        User testUser = new("Test", "User", "testuser@example.com");
        Order testOrder = new(testUser.Id);
        testOrder.SetCourier(Guid.NewGuid());

        _mockUserRepository
            .Setup(u => u.GetUserByHttpContext(It.IsAny<HttpContext>()))
            .ReturnsAsync(testUser);

        _mockOrderingRepository
            .Setup(o => o.GetOrderById(It.IsAny<Guid>()))
            .ReturnsAsync(testOrder);

        var result = await _courierOrderOverviewModel.OnGetAsync();

        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(UrlProvider.Courier.CourierDashboard, redirectResult.PageName);
        Assert.Equal(testUser, _courierOrderOverviewModel.CourierUser);
        Assert.Equal(testOrder, _courierOrderOverviewModel.Order);
        Assert.NotNull(_courierOrderOverviewModel.Order);
        Assert.NotNull(_courierOrderOverviewModel.CourierUser);
        Assert.NotEqual(testUser.Id, _courierOrderOverviewModel.Order.CourierId);
        _mockUserRepository.Verify(u => u.GetUserByHttpContext(It.IsAny<HttpContext>()), Times.Once);
        _mockOrderingRepository.Verify(o => o.GetOrderById(It.IsAny<Guid>()), Times.Once);
    }
}