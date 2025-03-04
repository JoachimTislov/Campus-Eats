using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Services;

public interface IFulfillmentService
{
    Task<Offer> CreateOffer(Guid orderId, Guid userId, decimal orderExpense);
    Task UpdateOffer(Offer offer);
    Task<Offer?> GetOfferByOrderId(Guid orderId);
    Task UpdateOfferToRefundable(Guid orderId, Order order);
    Task CancelOfferSinceOrderHasBeenShipped(Guid orderId, Order order);
    Task RefundOrderExpenseToCostumer(Guid orderId, Order order);
}