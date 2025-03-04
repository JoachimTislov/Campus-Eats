using MediatR;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;

public class ProductRepository(IDbRepository<FoodItem> dbRepository, IMediator mediator) : IProductRepository
{
    private readonly IDbRepository<FoodItem> _dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task<FoodItem> CreateFoodItem(string name, string description, decimal price, string? imageLink)
    {
        var foodItem = new FoodItem(name, description, price, imageLink);

        await _dbRepository.AddAsync(foodItem);

        await _mediator.Publish(new ProductCreated(foodItem.Id, name, description, price));

        return foodItem;
    }

    public async Task EditFoodItem(Guid itemId, string name, string description, decimal price, string? imageLink)
    {
        var foodItem = await GetFoodItemById(itemId);
        if (foodItem == null) return;

        foodItem.EditFoodItem(name, description, price, imageLink);

        await _dbRepository.Update(foodItem);

        if (foodItem.Stripe_productId != null)
        {
            await _mediator.Publish(new ProductEdited(foodItem.Stripe_productId, name, description, price));
        }
    }

    public async Task DeleteFoodItem(Guid itemId)
    {
        await _dbRepository.Remove(itemId);
    }

    public async Task<FoodItem?> GetFoodItemById(Guid itemId)
    {
        return await _dbRepository.GetByIdAsync(itemId);
    }

    public List<FoodItem> GetAllFoodItems()
    {
        return _dbRepository.All();
    }

    public async Task UpdateStripeProductId(Guid itemId, string stripe_productId)
    {
        var foodItem = await GetFoodItemById(itemId);
        if (foodItem == null) return;

        foodItem.Stripe_productId = stripe_productId;

        await _dbRepository.Update(foodItem);
    }
}