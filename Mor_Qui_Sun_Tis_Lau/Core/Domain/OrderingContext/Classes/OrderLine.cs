using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;

public class OrderLine : BaseEntity
{
    public OrderLine() { }

    public OrderLine(string name, decimal price, int quantity, Guid orderId, string stripe_productId)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
        OrderId = orderId;
        Stripe_productId = stripe_productId;
    }

    public Guid Id { get; protected set; }
    public Guid OrderId { get; private set; }
    public string Name { get; protected set; } = string.Empty;
    public decimal Price { get; protected set; }
    public int Quantity { get; protected set; }

    public string Stripe_productId { get; private set; } = string.Empty;
}