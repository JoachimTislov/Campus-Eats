using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Services;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.ProductCRUD.ViewModels;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Admin.ProductCRUD;

[Authorize(Roles = "Admin")]
public class CreateProductModel(IProductService productService) : PageModel
{
    private readonly IProductService _productService = productService ?? throw new ArgumentNullException(nameof(productService));

    [BindProperty]
    public FoodItemViewModel ViewModel { get; set; } = new();

    public List<string> Errors { get; set; } = [];

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (ViewModel!.ImageFile != null)
        {
            var (success, error) = _productService.ValidateImageFile(ViewModel.ImageFile);
            if (!success)
            {
                Errors.Add(error);
                return Page();
            }
        }

        await _productService.CreateFoodItemAsync(ViewModel);

        return RedirectToPage(UrlProvider.Index);
    }
}
