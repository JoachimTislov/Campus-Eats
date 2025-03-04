using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;

public class InvoicingRepository(IDbRepository<Invoice> dbRepository, IMediator mediator) : IInvoicingRepository
{
    private readonly IDbRepository<Invoice> _dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task CreateInvoice(Guid orderId, User customer, decimal PaymentDue, InvoiceAddress address)
    {
        var invoice = new Invoice(orderId, customer, PaymentDue, address);

        await _dbRepository.AddAsync(invoice);
    }

    public async Task UpdateInvoiceStatusById(Guid invoiceId, InvoiceStatusEnum status)
    {
        var invoice = await GetInvoiceById(invoiceId);
        if (invoice == null) return;

        invoice.SetStatus(status);
        await UpdateInvoice(invoice);

        if (status == InvoiceStatusEnum.Paid)
        {
            await _mediator.Publish(new InvoicePaid(invoice.OrderId));
        }
    }

    private async Task UpdateInvoice(Invoice invoice) => await _dbRepository.Update(invoice);

    public async Task HandleTransactionResult(bool transactionResult, Guid invoiceId)
    {
        await UpdateInvoiceStatusById(invoiceId, transactionResult ? InvoiceStatusEnum.Paid : InvoiceStatusEnum.TransactionFailed);
    }

    public async Task<Invoice?> GetInvoiceById(Guid invoiceId)
    {
        return await _dbRepository.GetByIdAsync(invoiceId);
    }

    public async Task<Invoice?> GetInvoiceByOrderId(Guid orderId)
    {
        return await _dbRepository.WhereFirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<Dictionary<string, List<Invoice>>> GetSortedInvoices()
    {
        var sorted_invoices = new Dictionary<string, List<Invoice>>();

        foreach (var invoice in await _dbRepository.IncludeToListAsync(i => i.Customer))
        {
            var key = invoice.Status.ToString();

            if (!sorted_invoices.TryGetValue(key, out _))
            {
                sorted_invoices[key] = [];
            }
            sorted_invoices[key].Add(invoice);
        }

        return sorted_invoices;
    }

    public async Task CreditInvoiceByOrderId(Guid orderId)
    {
        var invoice = await GetInvoiceByOrderId(orderId);
        if (invoice == null) return;

        invoice.CreditInvoice();

        await UpdateInvoice(invoice);
    }

    public async Task CancelOrderPaymentOfInvoiceAndReplaceWithDeliveryFee(Guid orderId, decimal deliveryFee)
    {
        var invoice = await GetInvoiceByOrderId(orderId);
        if (invoice == null) return;

        invoice.SetPaymentDue(deliveryFee);

        await UpdateInvoice(invoice);
    }
}