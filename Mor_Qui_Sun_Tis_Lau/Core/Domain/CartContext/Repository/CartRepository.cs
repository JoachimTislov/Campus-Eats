using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Repository;

public class CartRepository(IDbRepository<Cart> dbRepository) : ICartRepository
{
    private readonly IDbRepository<Cart> _dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));

    private async Task<Cart> CreateCart(Guid cartId)
    {
        var cart = new Cart(cartId);
        await _dbRepository.AddAsync(cart);
        return cart;
    }

    private async Task<Cart?> GetCartByIdAsync(Guid cartId)
    {
        return await _dbRepository.IncludeWhereFirstOrDefaultAsync(c => c.Items, c => c.Id == cartId);
    }

    public async Task<Cart> GetCartAsync(Guid cartId)
    {
        var cart = await GetCartByIdAsync(cartId);
        cart ??= await CreateCart(cartId);

        return cart;
    }

    public async Task<List<Cart>> GetCartsWithSpecificItemAsync(Guid itemId)
    {
        return await _dbRepository.IncludeWhereToListAsync(c => c.Items, c => c.Items.Any(i => i.Sku == itemId));
    }

    public async Task UpdateCartAsync(Cart cart) => await _dbRepository.Update(cart);

    public async Task UpdateCartsAsync(List<Cart> carts) => await _dbRepository.UpdateRange(carts);

    public async Task AddItemToCartAsync(Guid itemId, string name, decimal price, int count, Guid cartId, string imageLink, string stripe_productId)
    {
        var cart = await GetCartAsync(cartId);
        cart.AddItem(itemId, name, price, count, imageLink, stripe_productId);
        await UpdateCartAsync(cart);
    }

    public async Task IncrementCartItemCountAsync(Guid itemId, Guid cartId)
    {
        var cart = await GetCartAsync(cartId);
        cart.IncrementCountOfItem(itemId);
        await UpdateCartAsync(cart);
    }

    public async Task DecrementCartItemCountOrDeleteIfZeroAsync(Guid itemId, Guid cartId)
    {
        var cart = await GetCartAsync(cartId);
        cart.RemoveItem(itemId);
        await UpdateCartAsync(cart);
    }

    public async Task DeleteCartItemFromCartAsync(Guid itemId, Guid cartId)
    {
        var cart = await GetCartAsync(cartId);
        cart.DeleteItemFromCart(itemId);
        await UpdateCartAsync(cart);
    }

    public async Task DeleteCartAsync(Guid cartId)
    {
        await _dbRepository.Remove(cartId);
    }
}