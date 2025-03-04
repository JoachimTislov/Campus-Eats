using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.ProductS;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.StripeContext.Handlers;

public class ProductEditedHandlerTests
{
    private readonly Mock<IStripeProductService> _mockStripeProductService = new();

    private readonly ProductEditedHandler _productDeletedHandler;

    public ProductEditedHandlerTests()
    {
        _productDeletedHandler = new(_mockStripeProductService.Object);
    }

    [Fact]
    public async Task Handle_ShouldExecuteEditProductById()
    {
        ProductEdited productEdited = new("stripe_productId", "name", "description", 10m);

        await _productDeletedHandler.Handle(productEdited, CancellationToken.None);

        _mockStripeProductService
            .Verify(m => m.EditProductById(
                productEdited.Stripe_productId,
                productEdited.Name,
                productEdited.Description,
                productEdited.Price,
                CancellationToken.None
            ), Times.Once);
    }
}