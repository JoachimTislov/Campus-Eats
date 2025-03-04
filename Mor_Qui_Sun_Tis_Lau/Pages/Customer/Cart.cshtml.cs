using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Services;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Customer;

[Authorize(Roles = "Customer")]
public class CartModel(ICartService cartService) : PageModel
{
    private readonly ICartService _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));

    public List<CartItem> CartItems { get; set; } = [];
    public decimal CartSubtotal { get; set; }

    private async Task<Guid> GetCartId() => await _cartService.GetCartIdFromSessionAsync(User, HttpContext.Session);

    private async Task LoadCart()
    {
        var cartId = await GetCartId();
        var cart = await _cartService.GetCartAsync(cartId);

        CartItems = cart.Items.ToList();
        CartSubtotal = await _cartService.GetCartSubtotal(cartId);
    }

    public async Task OnGetAsync()
    {
        await LoadCart();
    }

    public async Task<IActionResult> OnPostCheckoutCartAsync()
    {
        await LoadCart();

        Guid orderId = Guid.Empty;
        if (CartItems.Count > 0)
        {
            orderId = await _cartService.CheckoutCart(await GetCartId(), CartItems, User, HttpContext.Session);
        }

        if (orderId != Guid.Empty) return RedirectToPage(UrlProvider.Customer.Ordering, new { orderId });

        return RedirectToPage();
    }

    public async Task OnPostIncrementCartItemCountAsync(string itemId)
    {
        await _cartService.IncrementCartItemCountAsync(Guid.Parse(itemId), await GetCartId());
    }

    public async Task OnPostDecrementCartItemCountOrDeleteIfZeroAsync(string itemId)
    {
        await _cartService.DecrementCartItemCountOrDeleteIfZeroAsync(Guid.Parse(itemId), await GetCartId());
    }

    public async Task<IActionResult> OnPostDeleteCartItemFromCartAsync(string itemId)
    {
        await _cartService.DeleteCartItemFromCartAsync(Guid.Parse(itemId), await GetCartId());

        return RedirectToPage();
    }
}