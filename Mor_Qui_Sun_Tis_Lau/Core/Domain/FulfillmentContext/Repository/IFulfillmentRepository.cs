using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Repository;

public interface IFulfillmentRepository
{
    Task<Offer> CreateOffer(Guid orderId, Guid userId, decimal orderExpense);
    Task<Offer?> GetOfferByOrderId(Guid orderId);
    Task UpdateOffer(Offer offer);
}