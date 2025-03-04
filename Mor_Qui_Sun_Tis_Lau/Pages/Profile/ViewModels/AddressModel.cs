using System.ComponentModel.DataAnnotations;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Profile.ViewModels;

public class AddressViewModel
{
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}