
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.InvoicingContext;

public class InvoiceTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var invoice = new Invoice();

        Assert.Equal(Guid.Empty, invoice.Id);
        Assert.Equal(0, invoice.PaymentDue);
        Assert.IsType<User>(invoice.Customer);
        Assert.Equal(Guid.Empty, invoice.OrderId);
        Assert.Equal(InvoiceStatusEnum.Pending, invoice.Status);
    }

    [Fact]
    public void ConstructorWithValues_ShouldAssignValues()
    {
        var orderId = Guid.NewGuid();
        var customer = new User("firstName", "lastName", "email");
        var paymentDue = 1.0m;
        var address = new InvoiceAddress();

        var invoice = new Invoice(orderId, customer, paymentDue, address);

        Assert.Equal(Guid.Empty, invoice.Id);
        Assert.Equal(paymentDue, invoice.PaymentDue);
        Assert.Equal(address, invoice.Address);
        Assert.Equal(customer, invoice.Customer);
        Assert.Equal(orderId, invoice.OrderId);
        Assert.Equal(InvoiceStatusEnum.Pending, invoice.Status);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var orderId = Guid.NewGuid();
        var customer = new User("firstName", "lastName", "email");
        var paymentDue = 1.0m;
        var address = new InvoiceAddress();

        var invoice = new Invoice(orderId, customer, paymentDue, address);

        invoice.SetStatus(InvoiceStatusEnum.Paid);

        Assert.Equal(InvoiceStatusEnum.Paid, invoice.Status);

        invoice.CreditInvoice();

        Assert.Equal(Guid.Empty, invoice.Id);
        Assert.Equal(paymentDue, invoice.PaymentDue);
        Assert.Equal(address, invoice.Address);
        Assert.Equal(customer, invoice.Customer);
        Assert.Equal(orderId, invoice.OrderId);
        Assert.Equal(InvoiceStatusEnum.Credited, invoice.Status);
    }

    [Fact]
    public void Properties_AssertSetters()
    {
        var type = typeof(Invoice);

        AssertSetter.AssertPrivate(type, nameof(Invoice.Id));
        AssertSetter.AssertPrivate(type, nameof(Invoice.PaymentDue));
        AssertSetter.AssertPrivate(type, nameof(Invoice.Customer));
        AssertSetter.AssertPrivate(type, nameof(Invoice.OrderId));
        AssertSetter.AssertPrivate(type, nameof(Invoice.Status));
    }
}