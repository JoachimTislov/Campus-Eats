
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;

public class Invoice : BaseEntity
{
    public Invoice() { }

    public Invoice(Guid orderId, User customer, decimal paymentDue, InvoiceAddress address)
    {
        Customer = customer;
        PaymentDue = paymentDue;
        OrderId = orderId;
        Address = address;
    }

    public Guid Id { get; private set; }
    public decimal PaymentDue { get; private set; }
    public User Customer { get; private set; } = new();
    public Guid OrderId { get; private set; }
    public InvoiceAddress Address { get; private set; } = new();
    public InvoiceStatusEnum Status { get; private set; } = InvoiceStatusEnum.Pending;

    public void SetPaymentDue(decimal payment) => PaymentDue = payment;
    public void SetStatus(InvoiceStatusEnum status) => Status = status;
    public bool IsPayable() => Status == InvoiceStatusEnum.Pending || Status == InvoiceStatusEnum.TransactionFailed;
    public void CreditInvoice() => SetStatus(InvoiceStatusEnum.Credited);
}