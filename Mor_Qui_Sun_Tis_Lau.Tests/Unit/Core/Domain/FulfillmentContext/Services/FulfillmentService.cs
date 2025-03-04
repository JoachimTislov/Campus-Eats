using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.FulfillmentContext.Services;

public class FulfillmentServiceTests
{
    private readonly Mock<IFulfillmentRepository> _mockFulfillmentRepository = new();

    private readonly FulfillmentService _fulfillmentService;

    public FulfillmentServiceTests()
    {
        _fulfillmentService = new(_mockFulfillmentRepository.Object);
    }

    [Fact]
    public async Task CreateOffer()
    {
        await _fulfillmentService.CreateOffer(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>());

        _mockFulfillmentRepository.Verify(m => m.CreateOffer(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Once);
    }

    [Fact]
    public async Task GetOfferByOrderId()
    {
        await _fulfillmentService.GetOfferByOrderId(It.IsAny<Guid>());

        _mockFulfillmentRepository.Verify(m => m.GetOfferByOrderId(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task RefundOrderExpenseToCostumer_ShouldRefundOrderExpense_WhenOfferIsNullNot()
    {
        var offer = new Offer();
        _mockFulfillmentRepository
            .Setup(m => m.GetOfferByOrderId(It.IsAny<Guid>()))
            .ReturnsAsync(offer);

        await _fulfillmentService.RefundOrderExpenseToCostumer(It.IsAny<Guid>(), It.IsAny<Order>());

        Assert.Equal(OfferStatus.Paid, offer.Status);

        _mockFulfillmentRepository.Verify(m => m.UpdateOffer(offer), Times.Once);
    }

    [Fact]
    public async Task RefundOrderExpenseToCostumer_ShouldCreateNewOffer_WhenOfferIsNull()
    {
        var customerId = Guid.NewGuid();
        var order = new Order(customerId);

        Offer? offer = null;
        _mockFulfillmentRepository
            .Setup(m => m.GetOfferByOrderId(It.IsAny<Guid>()))
            .ReturnsAsync(offer);

        Offer newOffer = new();
        _mockFulfillmentRepository
            .Setup(m => m.CreateOffer(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()))
            .ReturnsAsync(newOffer);

        await _fulfillmentService.RefundOrderExpenseToCostumer(It.IsAny<Guid>(), order);

        _mockFulfillmentRepository.Verify(m => m.GetOfferByOrderId(It.IsAny<Guid>()), Times.Once);
        _mockFulfillmentRepository.Verify(m => m.CreateOffer(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Once);
        _mockFulfillmentRepository.Verify(m => m.UpdateOffer(It.IsAny<Offer>()), Times.Once);
    }
}