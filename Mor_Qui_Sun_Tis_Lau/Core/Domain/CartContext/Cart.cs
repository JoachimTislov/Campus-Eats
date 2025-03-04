using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;

public class Cart(Guid id) : BaseEntity
{
	public Guid Id { get; private set; } = id;

	private readonly List<CartItem> _items = [];
	public IEnumerable<CartItem> Items => _items.AsReadOnly();

	private CartItem? FindItem(Guid itemId) => _items.SingleOrDefault(item => item.Sku == itemId);

	public void AddItem(Guid itemId, string name, decimal price, int count, string imageLink, string stripe_productId)
	{
		var item = FindItem(itemId);
		if (item != null)
		{
			item.Count += count;
		}
		else
		{
			_items.Add(new(itemId, name, price, count, imageLink, stripe_productId));
		}
	}

	public void IncrementCountOfItem(Guid itemId) => FindItem(itemId)?.AddOne();

	public void RemoveItem(Guid itemId)
	{
		var item = FindItem(itemId);
		if (item == null) return;

		if (item.IsCountOne())
		{
			_items.Remove(item);
		}
		else
		{
			item.RemoveOne();
		}
	}

	public void DeleteItemFromCart(Guid itemId)
	{
		var item = FindItem(itemId);
		if (item == null) return;

		_items.Remove(item);
	}

	public decimal GetSubtotal()
	{
		var subtotal = 0m;

		foreach (var item in _items)
		{
			subtotal += item.Sum;
		}

		return subtotal;
	}
}