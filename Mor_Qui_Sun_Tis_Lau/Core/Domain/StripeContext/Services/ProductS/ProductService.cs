using Stripe;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.ProductS;

public class StripeProductService : IStripeProductService
{
    private readonly ProductService _productService = new();

    private async Task<string?> CheckIfProductIsCreated(string name, string description)
    {
        var products = await _productService.ListAsync(new ProductListOptions
        {
            Limit = 50,
        });

        var product = products.Data.FirstOrDefault(p => p.Name == name && p.Description == description);

        return product?.Id;
    }

    public async Task<string> CreateProduct(string name, string description, decimal price, bool checkIfProductIsCreated, CancellationToken cancellationToken)
    {
        if (checkIfProductIsCreated)
        {
            var productId = await CheckIfProductIsCreated(name, description);
            if (productId != null) return productId;
        }

        var product = await _productService.CreateAsync(new ProductCreateOptions
        {
            Name = name,
            Description = description,
            DefaultPriceData = new ProductDefaultPriceDataOptions()
            {
                Currency = "nok",
                UnitAmountDecimal = price * 100,
            },
            Active = true,
            Shippable = true

        }, cancellationToken: cancellationToken);

        return product.Id;
    }

    private static async Task<string> CreateNewPriceForProduct(string productId, decimal price, CancellationToken cancellationToken)
    {
        var priceCreateOptions = new PriceCreateOptions
        {
            UnitAmountDecimal = price * 100,
            Currency = "nok",
            Product = productId
        };

        return (await new PriceService().CreateAsync(priceCreateOptions, cancellationToken: cancellationToken)).Id;
    }

    public async Task EditProductById(string productId, string name, string description, decimal price, CancellationToken cancellationToken)
    {
        var productUpdateOptions = new ProductUpdateOptions()
        {
            Name = name,
            Description = description,
            DefaultPrice = await CreateNewPriceForProduct(productId, price, cancellationToken)

        };

        await _productService.UpdateAsync(productId, productUpdateOptions, cancellationToken: cancellationToken);
    }

    public async Task<Product> GetProductById(string productId)
    {
        return await _productService.GetAsync(productId);
    }
}