using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Customer;

[Authorize(Roles = "Customer")]
public class CanteenModel(ICartService cartService, IProductRepository productRepository) : PageModel
{
    private readonly ICartService _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

    public List<FoodItem> FoodItems { get; set; } = [];
    public FoodItem? SelectedItem { get; set; }

    public void OnGet()
    {
        LoadFoodItems();
    }

    private void LoadFoodItems()
    {
        FoodItems = _productRepository.GetAllFoodItems();
    }

    private async Task<FoodItem?> GetFoodItemById(Guid itemId) => await _productRepository.GetFoodItemById(itemId);

    public async Task<IActionResult> OnPostDisplayItemAsync(Guid itemId)
    {
        SelectedItem = await GetFoodItemById(itemId);

        return Partial("Partials/_ProductDetailsPartial", this);
    }

    public async Task<IActionResult> OnPostAddToCartAsync(Guid itemId, string name, decimal price, int count, string imageLink, string stripe_productId)
    {
        var cartId = await _cartService.GetCartIdFromSessionAsync(User, HttpContext.Session);

        await _cartService.AddItemToCartAsync(itemId, name, price, count, cartId, imageLink, stripe_productId);

        return RedirectToPage();
    }
}