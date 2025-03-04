using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;

public interface IOrderingRepository
{
    Task<Order> CreateOrder(Guid customerId);
    Task<Order?> GetOrderById(Guid orderId);
    Task UpdateOrder(Order order);
    Task SetOrderStatus(Guid orderId, OrderStatusEnum status);
    Task SetCourier(Guid orderId, Guid courierId);
    Task SetCampusLocation(Guid orderId, CampusLocation campusLocation);
    Task SetTip(Guid orderId, decimal tip);
    Task<List<Order>> GetOrdersByCustomerId(Guid customerId);
    Task<List<Order>> GetOrdersByOrderStatus(OrderStatusEnum OrderStatus);
    Task<List<Order>> GetOrdersByCourierId(Guid courierId);
    Task<List<Order>> GetAllOrders();
}