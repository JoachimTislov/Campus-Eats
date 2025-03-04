using System.Security.Claims;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Services;

public interface ICartService
{
    Task<Guid> GetCartIdFromSessionAsync(ClaimsPrincipal claimsPrincipal, ISession session);
    Task<Cart> GetCartAsync(Guid cartId);
    Task<List<Cart>> GetCartsWithSpecificItemAsync(Guid itemId);
    Task AddItemToCartAsync(Guid itemId, string name, decimal price, int count, Guid cartId, string imageLink, string stripe_productId);
    Task IncrementCartItemCountAsync(Guid itemId, Guid cartId);
    Task DecrementCartItemCountOrDeleteIfZeroAsync(Guid itemId, Guid cartId);
    Task DeleteCartItemFromCartAsync(Guid itemId, Guid cartId);
    Task UpdateCartsAsync(List<Cart> cart);
    Task<decimal> GetCartSubtotal(Guid cartId);
    Task DeleteCartAsync(Guid cartId, ClaimsPrincipal claimsPrincipal, ISession session);
    Task<Guid> CheckoutCart(Guid cartId, List<CartItem> cartItems, ClaimsPrincipal claimsPrincipal, ISession session);
}