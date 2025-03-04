using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using System.Security.Claims;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Repository;
using MediatR;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Services;

public class CartService(ICartRepository cartRepository, IUserRepository userRepository, IMediator mediator) : ICartService
{
    private readonly ICartRepository _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    private async Task<string> GetUniqueCartIdentifier(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userRepository.GetUserByClaimsPrincipal(claimsPrincipal);
        if (user == null) return string.Empty; // This return only happens in testing because of cookies or whatever stores the login, TODO: should redirect them to logout page 
        return $"{user.Id}-CartId";
    }

    public async Task<Guid> GetCartIdFromSessionAsync(ClaimsPrincipal claimsPrincipal, ISession session)
    {
        var cartIdentifier = await GetUniqueCartIdentifier(claimsPrincipal);
        var cartId = session.GetGuid(cartIdentifier);
        if (cartId == null)
        {
            cartId = Guid.NewGuid();
            session.SetString(cartIdentifier, cartId.Value.ToString());
        }
        return cartId.Value;
    }

    public async Task<Cart> GetCartAsync(Guid cartId)
    {
        return await _cartRepository.GetCartAsync(cartId);
    }

    public async Task<List<Cart>> GetCartsWithSpecificItemAsync(Guid itemId)
    {
        return await _cartRepository.GetCartsWithSpecificItemAsync(itemId);
    }

    public async Task AddItemToCartAsync(Guid itemId, string name, decimal price, int count, Guid cartId, string imageLink, string stripe_productId)
    {
        await _cartRepository.AddItemToCartAsync(itemId, name, price, count, cartId, imageLink, stripe_productId);
    }

    public async Task IncrementCartItemCountAsync(Guid itemId, Guid cartId)
    {
        await _cartRepository.IncrementCartItemCountAsync(itemId, cartId);
    }

    public async Task DecrementCartItemCountOrDeleteIfZeroAsync(Guid itemId, Guid cartId)
    {
        await _cartRepository.DecrementCartItemCountOrDeleteIfZeroAsync(itemId, cartId);
    }

    public async Task DeleteCartItemFromCartAsync(Guid itemId, Guid cartId)
    {
        await _cartRepository.DeleteCartItemFromCartAsync(itemId, cartId);
    }

    public async Task UpdateCartsAsync(List<Cart> carts)
    {
        await _cartRepository.UpdateCartsAsync(carts);
    }

    public async Task<decimal> GetCartSubtotal(Guid cartId)
    {
        return (await GetCartAsync(cartId)).GetSubtotal();
    }

    public async Task DeleteCartAsync(Guid cartId, ClaimsPrincipal claimsPrincipal, ISession session)
    {
        await _cartRepository.DeleteCartAsync(cartId);
        await RemoveCartIdFromSession(cartId, claimsPrincipal, session);
    }

    public async Task<Guid> CheckoutCart(Guid cartId, List<CartItem> cartItems, ClaimsPrincipal claimsPrincipal, ISession session)
    {
        var tcs = new TaskCompletionSource<Guid>();

        await _mediator.Publish(new CartCheckedOut(cartItems, claimsPrincipal, tcs.SetResult));

        var orderId = await tcs.Task;

        if (orderId != Guid.Empty) await DeleteCartAsync(cartId, claimsPrincipal, session);

        return orderId;
    }

    private async Task RemoveCartIdFromSession(Guid cartId, ClaimsPrincipal claimsPrincipal, ISession session)
    {
        var cartIdentifier = await GetUniqueCartIdentifier(claimsPrincipal);
        var currentCartId = session.GetGuid(cartIdentifier);
        if (currentCartId.HasValue && currentCartId.Value == cartId)
        {
            session.Remove(cartIdentifier);
        }
    }
}