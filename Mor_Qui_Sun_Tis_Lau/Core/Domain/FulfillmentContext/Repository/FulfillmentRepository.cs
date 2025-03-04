using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Repository;

public class FulfillmentRepository(IDbRepository<Offer> dbRepository) : IFulfillmentRepository
{
    private readonly IDbRepository<Offer> _dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));

    public async Task<Offer> CreateOffer(Guid orderId, Guid userId, decimal orderExpense)
    {
        var offer = new Offer(orderId, userId, orderExpense);

        await _dbRepository.AddAsync(offer);

        return offer;
    }

    public async Task<Offer?> GetOfferByOrderId(Guid orderId)
    {
        return await _dbRepository.WhereFirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task UpdateOffer(Offer offer)
    {
        await _dbRepository.Update(offer);
    }
}