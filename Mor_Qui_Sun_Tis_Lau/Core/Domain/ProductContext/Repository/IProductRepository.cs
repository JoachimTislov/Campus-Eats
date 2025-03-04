namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;

public interface IProductRepository
{
    Task<FoodItem> CreateFoodItem(string name, string description, decimal price, string? imageLink);
    Task EditFoodItem(Guid itemId, string name, string description, decimal price, string? imageLink);
    Task DeleteFoodItem(Guid itemId);
    Task<FoodItem?> GetFoodItemById(Guid itemId);
    List<FoodItem> GetAllFoodItems();
    Task UpdateStripeProductId(Guid productId, string stripe_productId);
}