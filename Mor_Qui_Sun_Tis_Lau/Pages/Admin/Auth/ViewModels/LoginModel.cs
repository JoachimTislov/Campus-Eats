using System.ComponentModel.DataAnnotations;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Admin.Auth.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string AdminName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
}