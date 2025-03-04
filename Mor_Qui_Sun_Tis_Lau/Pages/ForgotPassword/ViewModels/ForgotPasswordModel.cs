using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Mor_Qui_Sun_Tis_Lau.Pages.ForgotPassword.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Repeat Password is required")]
    public string Repeat_Password { get; set; } = string.Empty;

    public string ForgotPasswordAlertMessage { get; set; } = string.Empty;
    public IdentityError[] Errors { get; set; } = [];

    public bool AlertSuccess { get; set; }
}