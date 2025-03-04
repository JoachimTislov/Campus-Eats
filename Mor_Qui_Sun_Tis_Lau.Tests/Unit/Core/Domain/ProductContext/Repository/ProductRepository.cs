using MediatR;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.ProductContext.Repository;

public class ProductRepositoryTests
{
    private readonly Mock<IDbRepository<FoodItem>> _mockDbRepository = new();
    private readonly Mock<IMediator> _mockMediator = new();

    private readonly ProductRepository _productRepository;

    public ProductRepositoryTests()
    {
        _productRepository = new ProductRepository(_mockDbRepository.Object, _mockMediator.Object);
    }

    [Fact]
    public async Task CreateFoodItem()
    {
        var foodItem = await _productRepository.CreateFoodItem(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>());

        _mockDbRepository.Verify(m => m.AddAsync(It.IsAny<FoodItem>()), Times.Once);
        _mockMediator.Verify(m => m.Publish(It.Is<ProductCreated>(p => p.ProductId == foodItem.Id), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task EditFoodItem_ShouldReturn_WhenFoodItemIsNull()
    {
        FoodItem? foodItem = null;
        _mockDbRepository
            .Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(foodItem);

        await _productRepository.EditFoodItem(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>());

        _mockDbRepository.Verify(m => m.Update(It.IsAny<FoodItem>()), Times.Never);
        _mockMediator.Verify(m => m.Publish(It.IsAny<ProductEdited>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task EditFoodItem_ShouldNotPublishProductEditedEvent_WhenStripeProductIdIsNull()
    {
        FoodItem foodItem = new(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>());
        _mockDbRepository
            .Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(foodItem);

        await _productRepository.EditFoodItem(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>());

        _mockDbRepository.Verify(m => m.Update(foodItem), Times.Once);
        _mockMediator.Verify(m => m.Publish(It.IsAny<ProductEdited>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task EditFoodItem_ShouldPublishProductEditedEvent_WhenStripeProductIdIsNotNull()
    {
        FoodItem foodItem = new(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>())
        {
            Stripe_productId = "stripe_productId"
        };

        _mockDbRepository
            .Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(foodItem);

        await _productRepository.EditFoodItem(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>());

        _mockDbRepository.Verify(m => m.Update(foodItem), Times.Once);
        _mockMediator.Verify(m => m.Publish(It.Is<ProductEdited>(p => p.Stripe_productId == foodItem.Stripe_productId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteFoodItem_ShouldExecuteRemoveInDbRepository()
    {
        await _productRepository.DeleteFoodItem(It.IsAny<Guid>());

        _mockDbRepository.Verify(m => m.Remove(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetFoodItemById()
    {
        await _productRepository.GetFoodItemById(It.IsAny<Guid>());

        _mockDbRepository.Verify(m => m.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public void GetAllFoodItems()
    {
        _productRepository.GetAllFoodItems();

        _mockDbRepository.Verify(m => m.All(), Times.Once);
    }

    [Fact]
    public async Task UpdateStripeProductId_ShouldNotAssignStripeProductId_WhenFoodItemIsNull()
    {
        FoodItem? foodItem = null;
        _mockDbRepository
            .Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(foodItem);

        await _productRepository.UpdateStripeProductId(It.IsAny<Guid>(), It.IsAny<string>());

        _mockDbRepository.Verify(m => m.Update(It.IsAny<FoodItem>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStripeProductId_ShouldAssignStripeProductId_WhenFoodItemIsNotNull()
    {
        var stripe_productId = "stripe_productId";

        FoodItem foodItem = new(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>());
        _mockDbRepository
            .Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(foodItem);

        await _productRepository.UpdateStripeProductId(It.IsAny<Guid>(), stripe_productId);

        Assert.Equal(stripe_productId, foodItem.Stripe_productId);

        _mockDbRepository.Verify(m => m.Update(It.IsAny<FoodItem>()), Times.Once);
    }
}