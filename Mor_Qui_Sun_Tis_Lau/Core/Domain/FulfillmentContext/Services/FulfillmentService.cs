
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Services;

public class FulfillmentService(IFulfillmentRepository fulfillmentRepository) : IFulfillmentService
{
    private readonly IFulfillmentRepository _fulfillmentRepository = fulfillmentRepository ?? throw new ArgumentNullException(nameof(fulfillmentRepository));

    public async Task<Offer> CreateOffer(Guid orderId, Guid userId, decimal orderExpense)
    {
        return await _fulfillmentRepository.CreateOffer(orderId, userId, orderExpense);
    }

    public async Task UpdateOffer(Offer offer)
    {
        await _fulfillmentRepository.UpdateOffer(offer);
    }

    public async Task<Offer?> GetOfferByOrderId(Guid orderId)
    {
        return await _fulfillmentRepository.GetOfferByOrderId(orderId);
    }

    public async Task UpdateOfferToRefundable(Guid orderId, Order order)
    {
        var offer = await GetOfferByOrderId(orderId);
        offer ??= await CreateOffer(orderId, order.CustomerId, order.TotalCost());

        offer.Refundable();
        await UpdateOffer(offer);
    }

    public async Task CancelOfferSinceOrderHasBeenShipped(Guid orderId, Order order)
    {
        var offer = await GetOfferByOrderId(orderId);
        offer ??= await CreateOffer(orderId, order.CustomerId, order.TotalCost());

        if (!offer.IsRefunded())
        {
            offer.CancelOffer();
            await UpdateOffer(offer);
        }
    }

    public async Task RefundOrderExpenseToCostumer(Guid orderId, Order order)
    {
        var offer = await GetOfferByOrderId(orderId);
        offer ??= await CreateOffer(orderId, order.CustomerId, order.TotalCost());

        /* TODO: Refunding money to user, this needs more logic to able to track the money */

        if (!offer.IsCanceled())
        {
            offer.RefundOrderExpenseToCostumer();
            await UpdateOffer(offer);
        }
    }
}