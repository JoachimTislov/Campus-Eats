using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;

public class CartItem : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Guid Sku { get; private set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Sum => Price * Count;
    public int Count { get; set; }
    public string ImageLink { get; set; } = string.Empty;
    public string Stripe_productId { get; set; } = string.Empty;

    private CartItem() { }

    public CartItem(Guid sku, string name, decimal price, int itemCount, string imageLink, string stripe_productId)
    {
        Sku = sku;
        Name = name;
        Price = price;
        Count = itemCount;
        ImageLink = imageLink;
        Stripe_productId = stripe_productId;
    }

    public void AddOne() => Count++;
    public void RemoveOne() => Count--;
    public bool IsCountOne() => Count <= 1;
}