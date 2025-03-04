using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext;

public class Offer : BaseEntity
{
    public Offer() { }

    public Offer(Guid orderId, Guid customerId, decimal orderExpense)
    {
        OrderId = orderId;
        CustomerId = customerId;
        OrderExpense = orderExpense;
    }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal OrderExpense { get; private set; }
    public OfferStatus Status { get; private set; } = OfferStatus.Unpaid;

    private bool CheckStatus(OfferStatus status) => Status == status;
    public bool IsRefundable() => CheckStatus(OfferStatus.Refundable);
    public bool IsRefunded() => CheckStatus(OfferStatus.Paid);
    public bool IsCanceled() => CheckStatus(OfferStatus.Canceled);

    private void SetStatus(OfferStatus status) => Status = status;
    public void Refundable() => SetStatus(OfferStatus.Refundable);
    public void CancelOffer() => SetStatus(OfferStatus.Canceled);
    public void RefundOrderExpenseToCostumer() => SetStatus(OfferStatus.Paid);
}