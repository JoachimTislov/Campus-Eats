using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;

public record FoodItemNameChanged(Guid ItemId, string OldName, string NewName) : BaseDomainEvent;
public record FoodItemPriceChanged(Guid ItemId, decimal OldPrice, decimal NewPrice) : BaseDomainEvent;

public record ProductCreated(Guid ProductId, string Name, string Description, decimal Price) : BaseDomainEvent;
public record ProductEdited(string Stripe_productId, string Name, string Description, decimal Price) : BaseDomainEvent;