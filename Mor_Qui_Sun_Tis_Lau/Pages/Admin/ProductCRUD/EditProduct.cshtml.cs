using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Services;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.ProductCRUD.ViewModels;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Admin.ProductCRUD;

[Authorize(Roles = "Admin")]
public class EditProductModel(IProductService productService) : PageModel
{
    private readonly IProductService _productService = productService ?? throw new ArgumentNullException(nameof(productService));

    [BindProperty]
    public FoodItemViewModel ViewModel { get; set; } = new();

    public List<string> Errors { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(Guid productId)
    {
        return await InitializeFoodItemViewModel(productId);
    }

    private async Task<IActionResult> InitializeFoodItemViewModel(Guid productId)
    {
        var product = await _productService.GetFoodItemByIdAsync(productId);
        if (product is null) return RedirectToPage(UrlProvider.Index);

        ViewModel = new FoodItemViewModel(product);

        return Page();
    }

    public async Task<IActionResult> OnPostEditAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (ViewModel.ImageFile != null)
        {
            var (success, error) = _productService.ValidateImageFile(ViewModel.ImageFile);
            if (!success)
            {
                Errors.Add(error);
            }
        }

        await _productService.EditFoodItemAsync(ViewModel);

        return RedirectToPage(UrlProvider.Index);
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        await _productService.DeleteFoodItemAsync(ViewModel.Id);

        return RedirectToPage(UrlProvider.Index);
    }
}
