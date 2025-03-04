using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Stripe;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;

public interface IOrderingService
{
    Task<Order> CreateOrder(Guid customerId);
    Task<List<Order>> GetOrders();
    Task<Order?> GetOrderById(Guid id);
    Task PlaceOrder(Guid orderId, CampusLocation campusLocation);
    Task PickOrder(Guid orderId, Guid courierId);
    Task SetTipForOrder(Guid orderId, decimal tip);
    Task<List<Order>> GetOrderHistoryByUserId(Guid userId);
    Task<bool> CanCancelOrder(Guid orderId);
    Task<Dictionary<OrderStatusEnum, int>> GetQuantityOfOrdersInEachStage();
    Task SetOrderStatus(Guid orderId, OrderStatusEnum status, OrderStatusEnum? oldStatus = null);
    Task<(bool, decimal)> GetDeliveryFeeForGivenOrderByOrderId(Guid orderId);
    Task<List<Order>> GetMonthOrders();
}