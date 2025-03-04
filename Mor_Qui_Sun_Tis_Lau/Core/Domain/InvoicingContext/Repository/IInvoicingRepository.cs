using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;

public interface IInvoicingRepository
{
    Task CreateInvoice(Guid orderId, User customer, decimal PaymentDue, InvoiceAddress address);
    Task UpdateInvoiceStatusById(Guid invoiceId, InvoiceStatusEnum status);
    Task HandleTransactionResult(bool transactionResult, Guid invoiceId);
    Task<Invoice?> GetInvoiceById(Guid invoiceId);
    Task<Invoice?> GetInvoiceByOrderId(Guid orderId);
    Task<Dictionary<string, List<Invoice>>> GetSortedInvoices();
    Task CreditInvoiceByOrderId(Guid orderId);
    Task CancelOrderPaymentOfInvoiceAndReplaceWithDeliveryFee(Guid orderId, decimal deliveryFee);
}