using MediatR;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.OrderingContext.Services;

public class OrderingServiceTests
{
    private readonly Mock<IMediator> _mockMediatR = new();
    private readonly Mock<IOrderingRepository> _mockOrderingRepository;
    private readonly OrderingService _orderingService;

    private static List<Order> ReturnListWithNewOrder() => [new Order()];

    private static Dictionary<User, Order> UserOrder()
    {
        User user = new() { Email = "henry@test.com" };
        Order order = new(user.Id);
        return new Dictionary<User, Order> { { user, order } };
    }

    public OrderingServiceTests()
    {
        _mockOrderingRepository = new();
        _orderingService = new(_mockMediatR.Object, _mockOrderingRepository.Object);
    }

    [Fact]
    public async Task CreateOrder_ShouldCreateOrder()
    {
        var userOrder = UserOrder();
        var user = userOrder.First().Key;
        var expectedOrder = userOrder.First().Value;

        _mockOrderingRepository
            .Setup(o => o.CreateOrder(It.IsAny<Guid>()))
            .ReturnsAsync(expectedOrder);

        var order = await _orderingService.CreateOrder(user.Id);

        Assert.Equal(expectedOrder, order);
        _mockOrderingRepository.Verify(o => o.CreateOrder(It.Is<Guid>(r => r == user.Id)), Times.Once);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnAllOrders()
    {
        var expectedOrders = new List<Order>
        {
            new() { },
            new() { }
        };

        _mockOrderingRepository
            .Setup(o => o.GetAllOrders())
            .Returns(Task.FromResult(expectedOrders));

        var orders = await _orderingService.GetOrders();

        Assert.Equal(expectedOrders, orders);
        _mockOrderingRepository.Verify(o => o.GetAllOrders(), Times.Once);
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnOrder()
    {
        var userOrder = UserOrder();
        var user = userOrder.First().Key;
        var expectedOrder = userOrder.First().Value;

        _mockOrderingRepository
            .Setup(o => o.GetOrderById(It.IsAny<Guid>()))
            .ReturnsAsync(expectedOrder);

        var order = await _orderingService.GetOrderById(expectedOrder.Id);

        Assert.Equal(expectedOrder, order);
        _mockOrderingRepository.Verify(o => o.GetOrderById(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetOrderById_ShouldFailToReturnOrder()
    {
        var orderId = Guid.NewGuid();

        _mockOrderingRepository
            .Setup(o => o.GetOrderById(It.IsAny<Guid>()))
            .ReturnsAsync((Order?)null);

        var order = await _orderingService.GetOrderById(orderId);

        Assert.Null(order);
        _mockOrderingRepository.Verify(o => o.GetOrderById(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetOrderHistoryByUserId_ShouldGetAllOrderHistory()
    {
        var userOrder = UserOrder();
        var user = userOrder.First().Key;
        var order = userOrder.First().Value;

        var expectedOrders = new List<Order>
        {
            new(user.Id),
            new(user.Id),
            new(user.Id)
        };

        _mockOrderingRepository
            .Setup(o => o.GetOrdersByCustomerId(It.IsAny<Guid>()))
            .ReturnsAsync(expectedOrders);

        var orderHistory = await _orderingService.GetOrderHistoryByUserId(user.Id);

        Assert.Equal(expectedOrders, orderHistory);
        _mockOrderingRepository.Verify(o => o.GetOrdersByCustomerId(It.IsAny<Guid>()), Times.Once);
    }

    private static List<Order> CreateListOfOrders(OrderStatusEnum[] orderStatusEnums)
    {
        List<Order> orders = [];
        foreach (var _status in orderStatusEnums)
        {
            var order = new Order();
            order.SetStatus(_status);
            orders.Add(order);
        }
        return orders;
    }

    [Fact]
    public async Task GetQuantityOfOrdersInEachStage_ShouldGetAllQuantityOfOrdersInEachStage()
    {
        var orderStatusEnums = new[]
        {
             OrderStatusEnum.New,
             OrderStatusEnum.Placed,
             OrderStatusEnum.Picked,
             OrderStatusEnum.Shipped,
             OrderStatusEnum.Delivered,
             OrderStatusEnum.Missing,
             OrderStatusEnum.Canceled,
        };

        var allOrders = CreateListOfOrders(orderStatusEnums);

        _mockOrderingRepository
            .Setup(s => s.GetAllOrders())
            .ReturnsAsync(allOrders);

        var expectedOrdersQuantity = new Dictionary<OrderStatusEnum, int>
        {
            { OrderStatusEnum.New, 1 },
            { OrderStatusEnum.Placed, 1 },
            { OrderStatusEnum.Picked, 1 },
            { OrderStatusEnum.Shipped, 1 },
            { OrderStatusEnum.Delivered, 1 },
            { OrderStatusEnum.Missing, 1 },
            { OrderStatusEnum.Canceled, 1 }
        };

        var ordersQuantityInEachStage = await _orderingService.GetQuantityOfOrdersInEachStage();

        Assert.Equal(expectedOrdersQuantity, ordersQuantityInEachStage);
        _mockOrderingRepository.Verify(s => s.GetAllOrders(), Times.Once);
    }
}