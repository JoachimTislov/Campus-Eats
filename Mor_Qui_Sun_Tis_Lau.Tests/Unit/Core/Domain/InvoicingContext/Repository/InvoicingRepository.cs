using MediatR;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.InvoicingContext.Repository;

public class InvoicingRepositoryTests
{
    private readonly Mock<IDbRepository<Invoice>> _mockDbRepository = new();
    private readonly Mock<IMediator> _mockMediator = new();

    private readonly InvoicingRepository _invoicingRepository;

    public InvoicingRepositoryTests()
    {
        _invoicingRepository = new(_mockDbRepository.Object, _mockMediator.Object);
    }

    [Fact]
    public async Task CreateInvoice()
    {
        await _invoicingRepository.CreateInvoice(It.IsAny<Guid>(), new User(), It.IsAny<decimal>(), It.IsAny<InvoiceAddress>());

        _mockDbRepository.Verify(m => m.AddAsync(It.IsAny<Invoice>()), Times.Once);
    }

    [Fact]
    public async Task UpdateInvoiceStatusById_ShouldReturn_WhenInvoiceIsNull()
    {
        Invoice? invoice = null;
        _mockDbRepository
            .Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(invoice);

        await _invoicingRepository.UpdateInvoiceStatusById(It.IsAny<Guid>(), It.IsAny<InvoiceStatusEnum>());

        _mockDbRepository.Verify(m => m.GetByIdAsync(It.IsAny<Guid>()), Times.Once);

        _mockDbRepository.Verify(m => m.Update(It.IsAny<Invoice>()), Times.Never);
    }

    [Fact]
    public async Task UpdateInvoiceStatusById_ShouldUpdate_WhenInvoiceIsNotNull()
    {
        var invoiceStatus = InvoiceStatusEnum.TransactionFailed;
        var invoice = new Invoice();
        _mockDbRepository
            .Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(invoice);

        await _invoicingRepository.UpdateInvoiceStatusById(It.IsAny<Guid>(), invoiceStatus);

        Assert.Equal(invoice.Status, invoiceStatus);

        _mockDbRepository.Verify(m => m.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockDbRepository.Verify(m => m.Update(invoice), Times.Once);
    }

    [Fact]
    public async Task HandleTransactionResult_ShouldPublishEvent_WhenTransactionIsSuccessful()
    {
        var invoice = new Invoice();
        _mockDbRepository
            .Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(invoice);

        await _invoicingRepository.HandleTransactionResult(true, Guid.NewGuid());

        _mockDbRepository.Verify(m => m.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockDbRepository.Verify(m => m.Update(invoice), Times.Once);
    }

    [Fact]
    public async Task HandleTransactionResult_ShouldNotPublishEvent_WhenTransactionIsNotSuccessful()
    {
        var invoice = new Invoice();
        _mockDbRepository
            .Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(invoice);

        await _invoicingRepository.HandleTransactionResult(false, Guid.NewGuid());

        _mockDbRepository.Verify(m => m.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockDbRepository.Verify(m => m.Update(invoice), Times.Once);
    }

    [Fact]
    public async Task GetInvoiceById()
    {
        var invoiceId = Guid.NewGuid();

        await _invoicingRepository.GetInvoiceById(invoiceId);

        _mockDbRepository.Verify(m => m.GetByIdAsync(invoiceId), Times.Once);
    }

    [Fact]
    public async Task GetInvoiceByOrderId()
    {
        var orderId = Guid.NewGuid();

        await _invoicingRepository.GetInvoiceByOrderId(orderId);

        _mockDbRepository.Verify(m => m.WhereFirstOrDefaultAsync(o => o.OrderId == orderId), Times.Once);
    }

    [Fact]
    public async Task GetSortedInvoices()
    {
        List<Invoice> invoices = [];

        for (var i = 0; i < 3; i++)
        {
            invoices.Add(new Invoice());
        }

        _mockDbRepository
            .Setup(m => m.IncludeToListAsync(i => i.Customer))
            .ReturnsAsync(invoices);

        var sorted_invoices = await _invoicingRepository.GetSortedInvoices();

        _mockDbRepository.Verify(m => m.IncludeToListAsync(i => i.Customer), Times.Once);

        Assert.IsType<Dictionary<string, List<Invoice>>>(sorted_invoices);
        Assert.Equal(3, sorted_invoices["Pending"].Count);
    }

    [Fact]
    public async Task CreditInvoiceByOrderId_ShouldReturn_WhenInvoiceIsNull()
    {
        var orderId = Guid.NewGuid();
        Invoice? invoice = null;
        _mockDbRepository
            .Setup(m => m.WhereFirstOrDefaultAsync(o => o.OrderId == orderId))
            .ReturnsAsync(invoice);

        await _invoicingRepository.CreditInvoiceByOrderId(orderId);

        _mockDbRepository.Verify(m => m.WhereFirstOrDefaultAsync(o => o.OrderId == orderId), Times.Once);
    }

    [Fact]
    public async Task CreditInvoiceByOrderId_ShouldCreditInvoiceAndAddEventToList_WhenInvoiceIsNotNull()
    {
        var orderId = Guid.NewGuid();
        var invoice = new Invoice();
        _mockDbRepository
            .Setup(m => m.WhereFirstOrDefaultAsync(o => o.OrderId == orderId))
            .ReturnsAsync(invoice);

        await _invoicingRepository.CreditInvoiceByOrderId(orderId);

        _mockDbRepository.Verify(m => m.WhereFirstOrDefaultAsync(o => o.OrderId == orderId), Times.Once);

        Assert.Equal(InvoiceStatusEnum.Credited, invoice.Status);
    }
}