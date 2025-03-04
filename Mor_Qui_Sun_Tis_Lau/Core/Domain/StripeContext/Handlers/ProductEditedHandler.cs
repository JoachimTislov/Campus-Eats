using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.ProductS;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Handlers;

public class ProductEditedHandler(IStripeProductService stripeProductService) : INotificationHandler<ProductEdited>
{
    private readonly IStripeProductService _stripeProductService = stripeProductService ?? throw new ArgumentNullException(nameof(stripeProductService));

    public async Task Handle(ProductEdited notification, CancellationToken cancellationToken)
    {
        var (stripe_productId, name, description, price) = notification;

        await _stripeProductService.EditProductById(stripe_productId, name, description, price, cancellationToken);
    }
}