using System.ComponentModel.DataAnnotations;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Profile.ViewModels;

public class ProfileViewModel
{
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}