using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Home.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is Required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Repeat Password is required")]
    public string Repeat_Password { get; set; } = string.Empty;

    public string RegisterErrorMessage { get; set; } = string.Empty;

    public IdentityError[] Errors { get; set; } = [];
}