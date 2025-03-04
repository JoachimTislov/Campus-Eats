
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.FulfillmentContext.Repository;

public class FulfillmentRepositoryTests
{
    private readonly Mock<IDbRepository<Offer>> _mockDbRepository = new();

    private readonly FulfillmentRepository _fulfillmentRepository;

    public FulfillmentRepositoryTests()
    {
        _fulfillmentRepository = new(_mockDbRepository.Object);
    }

    [Fact]
    public async Task CreateOffer()
    {
        await _fulfillmentRepository.CreateOffer(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>());

        _mockDbRepository.Verify(m => m.AddAsync(It.IsAny<Offer>()), Times.Once);
    }

    [Fact]
    public async Task GetOfferByOrderId()
    {
        var orderId = Guid.NewGuid();

        await _fulfillmentRepository.GetOfferByOrderId(orderId);

        _mockDbRepository.Verify(m => m.WhereFirstOrDefaultAsync(o => o.OrderId == orderId), Times.Once);
    }

    [Fact]
    public async Task UpdateOffer()
    {
        await _fulfillmentRepository.UpdateOffer(It.IsAny<Offer>());

        _mockDbRepository.Verify(m => m.Update(It.IsAny<Offer>()), Times.Once);
    }
}