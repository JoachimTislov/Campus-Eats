
namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Repository;

public interface ICartRepository
{
    Task<Cart> GetCartAsync(Guid cartId);
    Task<List<Cart>> GetCartsWithSpecificItemAsync(Guid itemId);
    Task UpdateCartAsync(Cart cart);
    Task UpdateCartsAsync(List<Cart> carts);
    Task AddItemToCartAsync(Guid itemId, string name, decimal price, int count, Guid cartId, string imageLink, string stripe_productId);
    Task IncrementCartItemCountAsync(Guid itemId, Guid cartId);
    Task DecrementCartItemCountOrDeleteIfZeroAsync(Guid itemId, Guid cartId);
    Task DeleteCartItemFromCartAsync(Guid itemId, Guid cartId);
    Task DeleteCartAsync(Guid cartId);
}