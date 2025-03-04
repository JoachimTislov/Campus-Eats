using System.ComponentModel.DataAnnotations;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Customer.ViewModels;

public class InvoiceAddressViewModel
{
    [Required(ErrorMessage = "Address Line is required")]
    [MinLength(1, ErrorMessage = "Address Line must be at least 1 characters long")]
    [MaxLength(30, ErrorMessage = "Address Line cannot exceed 30 characters")]
    public string AddressLine { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    [MinLength(1, ErrorMessage = "City must be at least 1 characters long")]
    [MaxLength(30, ErrorMessage = "City cannot exceed 30 characters")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Postal is required")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Must be exactly 4 digits")]
    public string PostalCode { get; set; } = string.Empty;

    public InvoiceAddressViewModel() { }

    public InvoiceAddressViewModel(Address address)
    {
        AddressLine = address.AddressLine;
        City = address.City;
        PostalCode = address.PostalCode;
    }
}