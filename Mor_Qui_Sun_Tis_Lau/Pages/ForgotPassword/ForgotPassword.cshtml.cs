using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.ForgotPassword.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Pages.ForgotPassword;

public class ForgotPasswordModel(IUserRepository userRepository, IUserService userService, IEmailService emailService, ILogger<ForgotPasswordModel> logger) : PageModel
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly ILogger<ForgotPasswordModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [BindProperty(SupportsGet = true)]
    public string? Token { get; set; }

    [EmailAddress, BindProperty(SupportsGet = true)]
    public string EmailOrUsername { get; set; } = string.Empty;

    [BindProperty]
    public ForgotPasswordViewModel ViewModel { get; set; } = new();

    private async Task<User?> GetUser(string emailOrUsername)
    {
        return await _userRepository.GetUserByEmail(emailOrUsername)
            ?? await _userRepository.GetUserByName(emailOrUsername);
    }

    public async Task<IActionResult> OnPostSendResetLinkAsync()
    {
        var user = await GetUser(EmailOrUsername);
        var (success, token) = await _userService.GeneratePasswordResetTokenAsync(user);
        if (!success || EmailOrUsername == null || !EmailOrUsername.Contains('@'))
        {
            ViewModel.ForgotPasswordAlertMessage = "Invalid Email";
        }
        else
        {
            var resetLink = $"{Request.Scheme}://{Request.Host}/Forgot-password?token={Uri.EscapeDataString(token)}&emailOrUsername={EmailOrUsername}";
            var result = await _emailService.SendMailAsync(EmailOrUsername, "Reset Password", $"Reset password: {resetLink}");
            if (result)
            {
                ViewModel.AlertSuccess = true;
                ViewModel.ForgotPasswordAlertMessage = "Success! Check your email";
                _logger.LogInformation("Link to reset password for account with email: {Email} sent successfully", EmailOrUsername);
            }
            else
            {
                ViewModel.ForgotPasswordAlertMessage = "Invalid Email";
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync(string token, string emailOrUsername)
    {
        if (ViewModel.Password != ViewModel.Repeat_Password)
        {
            ViewModel.ForgotPasswordAlertMessage = "Passwords do not match";
            return Page();
        }

        var user = await GetUser(emailOrUsername);
        var (success, userNotFound, errors) = await _userService.ChangePassword(user, Uri.UnescapeDataString(token), ViewModel.Password);
        if (userNotFound)
        {
            ViewModel.ForgotPasswordAlertMessage = "Invalid change-password attempt";
        }
        else if (success)
        {
            var userIsAdmin = await _userService.CheckIfUserIsAssignedRole(user, "Admin");
            if (userIsAdmin) return RedirectToPage(UrlProvider.Admin.Login);

            // The token will be invalid after one use
            Token = null; // Reset the process of changing the password
            ViewModel.AlertSuccess = true;
            ViewModel.ForgotPasswordAlertMessage = "Successfully changed your password!";
        }
        else
        {
            ViewModel.Errors = errors;
        }

        return Page();
    }
}
