using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.Auth.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Admin.Auth;

public class AdminLoginModel(IAdminService adminService, ILogger<AdminLoginModel> logger) : PageModel
{
    private readonly IAdminService _adminService = adminService;
    private readonly ILogger<AdminLoginModel> _logger = logger;

    [BindProperty]
    public LoginViewModel ViewModel { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        var (succeeded, errorMessage) = await _adminService.Login(ViewModel.AdminName, ViewModel.Password);
        if (succeeded)
        {
            _logger.LogInformation("Admin: {name} logged in", ViewModel.AdminName);
            return RedirectToPage(UrlProvider.Index);
        }
        else
        {
            if (errorMessage != "Change Password")
            {
                ViewModel.ErrorMessage = errorMessage;
                return Page();
            }

            var (success, token) = await _adminService.GeneratePasswordResetTokenAsync(ViewModel.AdminName);
            if (!success)
            {
                ViewModel.ErrorMessage = "Invalid admin name";
                return Page();
            }

            return RedirectToPage(UrlProvider.ForgotPassword, new { Token = token, EmailOrUsername = ViewModel.AdminName });
        }
    }
}