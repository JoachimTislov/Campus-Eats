using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Handlers;

public class FoodItemNameChangedHandler(ICartService cartService) : INotificationHandler<FoodItemNameChanged>
{
	private readonly ICartService _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));

	public async Task Handle(FoodItemNameChanged notification, CancellationToken cancellationToken)
	{
		var carts = await _cartService.GetCartsWithSpecificItemAsync(notification.ItemId);

		foreach (var cart in carts)
		{
			foreach (var item in cart.Items.Where(i => i.Sku == notification.ItemId))
			{
				item.Name = notification.NewName;
			}
		}

		await _cartService.UpdateCartsAsync(carts);
	}
}