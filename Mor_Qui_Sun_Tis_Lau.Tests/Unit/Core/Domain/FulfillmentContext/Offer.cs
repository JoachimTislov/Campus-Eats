
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.FulfillmentContext;

public class OfferTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var offer = new Offer();

        Assert.Equal(Guid.Empty, offer.Id);
        Assert.Equal(Guid.Empty, offer.OrderId);
        Assert.Equal(Guid.Empty, offer.CustomerId);
        Assert.Equal(0, offer.OrderExpense);
        Assert.Equal(OfferStatus.Unpaid, offer.Status);
    }

    [Fact]
    public void ConstructorWithValues_ShouldAssignValues()
    {
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var orderExpense = 10m;

        var offer = new Offer(orderId, customerId, orderExpense);

        Assert.Equal(Guid.Empty, offer.Id);
        Assert.Equal(orderId, offer.OrderId);
        Assert.Equal(customerId, offer.CustomerId);
        Assert.Equal(orderExpense, offer.OrderExpense);
        Assert.Equal(OfferStatus.Unpaid, offer.Status);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var orderExpense = 10m;

        var offer = new Offer(orderId, customerId, orderExpense);

        offer.RefundOrderExpenseToCostumer();

        Assert.Equal(Guid.Empty, offer.Id);
        Assert.Equal(orderId, offer.OrderId);
        Assert.Equal(customerId, offer.CustomerId);
        Assert.Equal(orderExpense, offer.OrderExpense);
        Assert.Equal(OfferStatus.Paid, offer.Status);
    }

    [Fact]
    public void Properties_AssertSetters()
    {
        var type = typeof(Offer);

        AssertSetter.AssertPrivate(type, nameof(Offer.Id));
        AssertSetter.AssertPrivate(type, nameof(Offer.OrderId));
        AssertSetter.AssertPrivate(type, nameof(Offer.CustomerId));
        AssertSetter.AssertPrivate(type, nameof(Offer.OrderExpense));
        AssertSetter.AssertPrivate(type, nameof(Offer.Status));
    }
}