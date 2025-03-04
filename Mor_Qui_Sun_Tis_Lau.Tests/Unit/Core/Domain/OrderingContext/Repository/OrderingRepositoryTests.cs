using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Infrastructure;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.OrderingContext.Repository;

public class OrderingRepositoryTests
{
    private readonly OrderingRepository _orderingRepository;
    private readonly ShopContext TestDb = DbTest.CreateContext();
    private readonly Mock<IAdminService> mockAdminService = new();

    public OrderingRepositoryTests()
    {
        var dbRepository = new DbRepository<Order>(TestDb);
        _orderingRepository = new OrderingRepository(dbRepository, mockAdminService.Object);
    }

    private static List<Order> CreateListOfOrders(OrderStatusEnum[] orderStatusEnums, Guid customerId)
    {
        List<Order> orders = [];
        foreach (var _status in orderStatusEnums)
        {
            var order = new Order(customerId);
            order.SetStatus(_status);
            orders.Add(order);
        }
        return orders;
    }

    [Fact]
    public async Task ExistingOrders_ReturnsListWithOrders()
    {
        TestDb.RemoveRange(TestDb.Orders);
        var customerId = Guid.NewGuid();
        var orders = new List<Order>
        {
            new(customerId),
            new(customerId),
            new(customerId)
        };

        orders[0].AddOrderLine("Item1", 20m, 1, "stripeId");
        orders[0].AddOrderLine("Item2", 20m, 1, "stripeId");

        TestDb.Orders.AddRange(orders);
        await TestDb.SaveChangesAsync();

        var result = await _orderingRepository.GetAllOrders();

        Assert.Equal(3, result.Count);
        Assert.Equal(2, result[0].OrderLines.Count());
        Assert.Equal(customerId, result[0].CustomerId);
    }

    [Fact]
    public async Task NoOrders_ReturnsEmptyList()
    {
        TestDb.RemoveRange(TestDb.Orders);
        var result = await _orderingRepository.GetAllOrders();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExistingOrders_ReturnsListWithOrdersMatchingStatus()
    {
        TestDb.RemoveRange(TestDb.Orders);
        var customerId = Guid.NewGuid();
        var customerId2 = Guid.NewGuid();

        var orderStatusEnums = new[]
        {
            OrderStatusEnum.New,
            OrderStatusEnum.Missing,
            OrderStatusEnum.Placed
        };

        List<Order> orders = CreateListOfOrders(orderStatusEnums, customerId2);

        orders[0].AddOrderLine("Item1", 20m, 1, "stripeId");
        orders[0].AddOrderLine("Item1", 20m, 1, "stripeId");

        TestDb.Orders.AddRange(orders);
        await TestDb.SaveChangesAsync();

        var resultCustomer1 = await _orderingRepository.GetOrdersByCustomerId(customerId);
        var resultCustomer2 = await _orderingRepository.GetOrdersByCustomerId(customerId2);

        Assert.NotEqual(customerId, customerId2);
        Assert.Equal(2, resultCustomer2[0].OrderLines.Count());
        Assert.Equal(3, resultCustomer2.Count);
        Assert.Empty(resultCustomer1);
    }

    [Fact]
    public async Task NoOrdersWithCustomerId_ReturnsEmptyList()
    {
        TestDb.RemoveRange(TestDb.Orders);
        var customerId = Guid.NewGuid();
        var customerId2 = Guid.NewGuid();

        var orderStatusEnums = new[]
        {
            OrderStatusEnum.Picked,
            OrderStatusEnum.Placed,
            OrderStatusEnum.Placed,
            OrderStatusEnum.Placed
        };

        List<Order> orders = CreateListOfOrders(orderStatusEnums, customerId2);

        TestDb.Orders.AddRange(orders);
        await TestDb.SaveChangesAsync();

        var result = await _orderingRepository.GetOrdersByCustomerId(customerId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task OrderExistsInDb_ReturnsOrder()
    {
        TestDb.RemoveRange(TestDb.Orders);
        var customerId = Guid.NewGuid();
        var order = new Order(customerId);
        order.AddOrderLine("Item1", 20m, 1, "stripeId");
        order.AddOrderLine("Item1", 20m, 1, "stripeId");

        TestDb.Orders.Add(order);
        await TestDb.SaveChangesAsync();

        var orderFromDb = await _orderingRepository.GetOrderById(order.Id);

        Assert.NotNull(orderFromDb);

        Assert.Equal(order, orderFromDb);
        Assert.Equal(2, orderFromDb.OrderLines.Count());
    }

    [Fact]
    public async Task OrderDoesNotExistInDb_ThrowsEntityNotFoundException()
    {
        TestDb.RemoveRange(TestDb.Orders);
        var result = await _orderingRepository.GetOrderById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Theory]
    [InlineData(OrderStatusEnum.New, 1)]
    [InlineData(OrderStatusEnum.Missing, 1)]
    [InlineData(OrderStatusEnum.Shipped, 1)]
    [InlineData(OrderStatusEnum.Delivered, 2)]
    [InlineData(OrderStatusEnum.Picked, 2)]
    [InlineData(OrderStatusEnum.Placed, 3)]
    public async Task ExistingOrders_GetOrderByOrderStatus_ReturnsListWithOrdersMatchingStatus(OrderStatusEnum status, int expectedCount)
    {
        TestDb.RemoveRange(TestDb.Orders);
        var customerId = Guid.NewGuid();

        // 1 New, Missing and Shipped, 2 Delivered and ReadyForPickup, 3 Placed
        var orderStatusEnums = new[]
        {
            OrderStatusEnum.New,
            OrderStatusEnum.Missing,
            OrderStatusEnum.Shipped,
            OrderStatusEnum.Delivered,
            OrderStatusEnum.Delivered,
            OrderStatusEnum.Picked,
            OrderStatusEnum.Picked,
            OrderStatusEnum.Placed,
            OrderStatusEnum.Placed,
            OrderStatusEnum.Placed
        };

        List<Order> orders = CreateListOfOrders(orderStatusEnums, customerId);

        TestDb.Orders.AddRange(orders);
        await TestDb.SaveChangesAsync();

        var result = await _orderingRepository.GetOrdersByOrderStatus(status);

        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public async Task NoOrdersWithStatusPlaced_ReturnsEmptyList()
    {
        TestDb.RemoveRange(TestDb.Orders);
        var customerId = Guid.NewGuid();

        var orderStatusEnums = new[]
        {
            OrderStatusEnum.New,
            OrderStatusEnum.Picked,
            OrderStatusEnum.Shipped
        };

        List<Order> orders = CreateListOfOrders(orderStatusEnums, customerId);

        TestDb.Orders.AddRange(orders);
        await TestDb.SaveChangesAsync();

        var result = await _orderingRepository.GetOrdersByOrderStatus(OrderStatusEnum.Placed);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateOrder_SuccessfullyCreatesOrder()
    {
        TestDb.RemoveRange(TestDb.Orders);
        var customerId = Guid.NewGuid();
        var createdOrder = await _orderingRepository.CreateOrder(customerId);

        Assert.NotNull(createdOrder);

        Assert.Equal(customerId, createdOrder.CustomerId);
    }

}