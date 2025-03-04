using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;

public class Order : BaseEntity
{
    public Order() { }

    public Order(Guid customerId)
    {
        CustomerId = customerId;
    }

    public Guid Id { get; protected set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public CampusLocation CampusLocation { get; set; } = new();
    public Guid CustomerId { get; private set; }
    public Guid? CourierId { get; private set; }
    public decimal Tip { get; private set; }
    public decimal DeliveryFee { get; private set; } = 50m;
    public decimal CourierDeliveryFeeCut => DeliveryFee * 0.8m;
    public decimal AdminDeliveryFeeCut => DeliveryFee * 0.2m;
    public decimal CourierEarning => CourierDeliveryFeeCut + Tip;
    public OrderStatusEnum Status { get; private set; } = OrderStatusEnum.New;
    private readonly List<OrderLine> _orders = [];
    public IEnumerable<OrderLine> OrderLines => _orders;

    public void SetDeliveryFee(decimal deliveryFee)
    {
        if (deliveryFee >= 0)
        {
            DeliveryFee = deliveryFee;
        }
    }

    public void SetTip(decimal tip)
    {
        if (tip >= 0)
        {
            Tip = tip;
        }
    }

    public void SetStatus(OrderStatusEnum status) => Status = status;

    public bool IsTipped() => Tip > 0;

    public decimal TotalCost()
    {
        return OrderLines.Sum(line => line.Price * line.Quantity) + DeliveryFee + Tip;
    }
    public decimal SubtotalCost()
    {
        return OrderLines.Sum(line => line.Price * line.Quantity);
    }

    public bool CanCancelOrder()
    {
        return CheckStatus(OrderStatusEnum.New) ||
           CheckStatus(OrderStatusEnum.Placed) ||
           CheckStatus(OrderStatusEnum.Picked);
    }

    public decimal TotalAdminBenefit()
    {
        return OrderLines.Sum(line => line.Price * line.Quantity) + AdminDeliveryFeeCut;
    }

    public void AddOrderLine(string orderName, decimal orderPrice, int orderAmount, string stripe_productId)
    {
        _orders.Add(new OrderLine(orderName, orderPrice, orderAmount, Id, stripe_productId));
    }

    private bool CheckStatus(OrderStatusEnum status) => Status == status;
    public bool IsNew() => CheckStatus(OrderStatusEnum.New);
    public bool IsCanceled() => CheckStatus(OrderStatusEnum.Canceled);
    public bool IsDelivered() => CheckStatus(OrderStatusEnum.Delivered);

    public void SetCourier(Guid courierId)
    {
        CourierId = courierId;
    }
}