using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.ProductS;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.StripeContext.Handlers;

public class ProductCreatedHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository = new();
    private readonly Mock<IStripeProductService> _mockStripeProductService = new();

    private readonly ProductCreatedHandler _productDeletedHandler;

    public ProductCreatedHandlerTests()
    {
        _productDeletedHandler = new(_mockProductRepository.Object, _mockStripeProductService.Object);
    }

    [Fact]
    public async Task Handle_ShouldExecuteCreateProduct_And_UpdateStripeProductIdWithProductRepository()
    {
        ProductCreated productCreated = new(Guid.NewGuid(), "name", "description", 10m);

        var stripe_productId = "stripe_productId";
        _mockStripeProductService
            .Setup(m => m.CreateProduct(productCreated.Name, productCreated.Description, productCreated.Price, It.IsAny<bool>(), CancellationToken.None))
            .ReturnsAsync(stripe_productId);

        await _productDeletedHandler.Handle(productCreated, CancellationToken.None);

        _mockStripeProductService
            .Verify(m => m.CreateProduct(
                productCreated.Name,
                productCreated.Description,
                productCreated.Price,
                It.IsAny<bool>(),
                CancellationToken.None
            ), Times.Once);

        _mockProductRepository.Verify(m => m.UpdateStripeProductId(productCreated.ProductId, stripe_productId), Times.Once);
    }
}