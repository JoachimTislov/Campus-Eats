using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;

public class OrderingService(IMediator mediator, IOrderingRepository orderingRepository) : IOrderingService
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IOrderingRepository _orderingRepository = orderingRepository ?? throw new ArgumentNullException(nameof(orderingRepository));

    public async Task<Order> CreateOrder(Guid customerId)
    {
        return await _orderingRepository.CreateOrder(customerId);
    }

    public async Task<List<Order>> GetOrders()
    {
        return await _orderingRepository.GetAllOrders();
    }

    public async Task<Order?> GetOrderById(Guid orderId)
    {
        return await _orderingRepository.GetOrderById(orderId);
    }

    public async Task PlaceOrder(Guid orderId, CampusLocation campusLocation)
    {
        await _orderingRepository.SetCampusLocation(orderId, campusLocation);

        await SetOrderStatus(orderId, OrderStatusEnum.Placed);
    }

    public async Task PickOrder(Guid orderId, Guid courierId)
    {
        await _orderingRepository.SetCourier(orderId, courierId);
        await SetOrderStatus(orderId, OrderStatusEnum.Picked);
    }

    public async Task SetTipForOrder(Guid orderId, decimal tip)
    {
        await _orderingRepository.SetTip(orderId, tip);
    }

    public async Task<List<Order>> GetOrderHistoryByUserId(Guid customerId)
    {
        return await _orderingRepository.GetOrdersByCustomerId(customerId);
    }

    public async Task<bool> CanCancelOrder(Guid orderId)
    {
        var order = await GetOrderById(orderId);
        if (order == null) return false;

        return order.CanCancelOrder();
    }

    public async Task<Dictionary<OrderStatusEnum, int>> GetQuantityOfOrdersInEachStage()
    {
        var quantityOfOrdersInEachStage = new Dictionary<OrderStatusEnum, int>();

        foreach (var order in await _orderingRepository.GetAllOrders())
        {
            if (!quantityOfOrdersInEachStage.TryGetValue(order.Status, out _))
            {
                quantityOfOrdersInEachStage[order.Status] = 1;
            }
            else
            {
                quantityOfOrdersInEachStage[order.Status]++;
            }
        }
        return quantityOfOrdersInEachStage;
    }

    public async Task SetOrderStatus(Guid orderId, OrderStatusEnum status, OrderStatusEnum? oldStatus = null)
    {
        await _orderingRepository.SetOrderStatus(orderId, status);

        await _mediator.Publish(new OrderStatusChanged(orderId, status, oldStatus));
    }

    public async Task<(bool, decimal)> GetDeliveryFeeForGivenOrderByOrderId(Guid orderId)
    {
        var order = await GetOrderById(orderId);
        if (order == null) return (false, 0);

        return (true, order.DeliveryFee);
    }

    public async Task<List<Order>> GetMonthOrders()
    {
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        var orders = await _orderingRepository.GetAllOrders();
        orders = orders.Where(o => o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear).ToList();

        return orders;
    }
}