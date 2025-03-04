using Microsoft.EntityFrameworkCore;
using Mor_Qui_Sun_Tis_Lau.Pages.Customer.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;

[Owned]
public class InvoiceAddress
{
    public InvoiceAddress() { }

    public InvoiceAddress(InvoiceAddressViewModel model)
    {
        AddressLine = model.AddressLine;
        City = model.City;
        PostalCode = model.PostalCode;
    }

    public string AddressLine { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}