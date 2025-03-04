using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.ProductS;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Handlers;

public class ProductCreatedHandler(IProductRepository productRepository, IStripeProductService stripeProductService) : INotificationHandler<ProductCreated>
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly IStripeProductService _stripeProductService = stripeProductService ?? throw new ArgumentNullException(nameof(stripeProductService));

    public async Task Handle(ProductCreated notification, CancellationToken cancellationToken)
    {
        var (productId, name, description, price) = notification;

        var stripe_productId = await _stripeProductService.CreateProduct(name, description, price, true, cancellationToken);

        await _productRepository.UpdateStripeProductId(productId, stripe_productId);
    }
}