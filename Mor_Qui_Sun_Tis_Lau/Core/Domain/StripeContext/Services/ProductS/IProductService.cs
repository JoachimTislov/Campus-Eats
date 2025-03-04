using Stripe;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.ProductS;

public interface IStripeProductService
{
    Task<string> CreateProduct(string name, string description, decimal price, bool checkIfProductIsCreated, CancellationToken cancellationToken);
    Task EditProductById(string productId, string name, string description, decimal price, CancellationToken cancellationToken);
    Task<Product> GetProductById(string productId);
}