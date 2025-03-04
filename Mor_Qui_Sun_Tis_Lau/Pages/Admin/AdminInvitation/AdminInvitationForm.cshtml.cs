using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Admin.AdminInvitation;

[Authorize(Roles = "Admin")]
public class AdminInvitationFormModel(IAdminService adminService) : PageModel
{
    private readonly IAdminService _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));

    [BindProperty(SupportsGet = true)]
    public Guid UserId { get; set; }

    [BindProperty]
    public string? Resume { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _adminService.SendAdminInvitation(UserId, Resume ?? string.Empty);

        return RedirectToPage(UrlProvider.Admin.InvitationOverview);
    }
}