using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;

public class OrderingRepository(IDbRepository<Order> dbRepository, IAdminService adminService) : IOrderingRepository
{
    private readonly IDbRepository<Order> _dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));
    private readonly IAdminService _adminService = adminService ?? throw new ArgumentNullException(nameof(dbRepository));

    public async Task<Order> CreateOrder(Guid customerId)
    {
        Order order = new(customerId);
        order.SetDeliveryFee(_adminService.GetDeliveryFee());

        await _dbRepository.AddAsync(order);

        return order;
    }

    public async Task<List<Order>> GetAllOrders()
    {
        return await _dbRepository.IncludeOrderByToListAsync(o => o.OrderLines, o => o.OrderDate);
    }

    public async Task UpdateOrder(Order order)
    {
        await _dbRepository.Update(order);
    }

    public async Task SetOrderStatus(Guid orderId, OrderStatusEnum status)
    {
        var order = await GetOrderById(orderId);
        if (order == null) return;

        order.SetStatus(status);

        await UpdateOrder(order);
    }

    public async Task SetCourier(Guid orderId, Guid courierId)
    {
        var order = await GetOrderById(orderId);
        if (order == null) return;

        order.SetCourier(courierId);

        await UpdateOrder(order);
    }

    public async Task SetCampusLocation(Guid orderId, CampusLocation campusLocation)
    {
        var order = await GetOrderById(orderId);
        if (order == null) return;

        order.CampusLocation = campusLocation;

        await UpdateOrder(order);
    }

    public async Task SetTip(Guid orderId, decimal tip)
    {
        var order = await GetOrderById(orderId);
        if (order == null) return;

        order.SetTip(tip);

        await UpdateOrder(order);
    }

    public async Task<Order?> GetOrderById(Guid id)
    {
        return await _dbRepository.IncludeWhereFirstOrDefaultAsync(o => o.OrderLines, o => o.Id == id);
    }

    public async Task<List<Order>> GetOrdersByCustomerId(Guid customerId)
    {
        return await _dbRepository.IncludeWhereOrderByToListAsync(o => o.OrderLines, o => o.CustomerId == customerId, o => o.OrderDate);
    }

    public async Task<List<Order>> GetOrdersByOrderStatus(OrderStatusEnum OrderStatus)
    {
        return await _dbRepository.IncludeWhereOrderByToListAsync(o => o.OrderLines, o => o.Status == OrderStatus, o => o.OrderDate);
    }

    public async Task<List<Order>> GetOrdersByCourierId(Guid courierId)
    {
        return await _dbRepository.IncludeWhereOrderByToListAsync(o => o.OrderLines, o => o.CourierId == courierId, o => o.OrderDate);
    }
}