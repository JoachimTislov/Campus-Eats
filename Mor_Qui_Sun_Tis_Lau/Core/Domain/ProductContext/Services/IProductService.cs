using Mor_Qui_Sun_Tis_Lau.Pages.Admin.ProductCRUD.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Services
{
    public interface IProductService
    {
        (bool, string) ValidateImageFile(IFormFile imageFile);
        Task CreateFoodItemAsync(FoodItemViewModel viewModel);
        Task<FoodItem?> GetFoodItemByIdAsync(Guid productId);
        Task EditFoodItemAsync(FoodItemViewModel viewModel);
        Task DeleteFoodItemAsync(Guid productId);
    }
}
