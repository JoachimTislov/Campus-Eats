using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Pages;

[Authorize(Roles = "Customer, Courier")]
public class RespondToAdminInvitationModel(IAdminService adminService, IUserRepository userRepository) : PageModel
{
    private readonly IAdminService _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    public User? ShopUser { get; private set; }

    [BindProperty(SupportsGet = true)]
    public Guid AdminInvitationId { get; set; }

    public AdminInvitation? AdminInvitation { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        return await LoadAdminInvitationAndUser();
    }

    private async Task<IActionResult> LoadAdminInvitationAndUser()
    {
        AdminInvitation = await _adminService.GetAdminInvitationByIdAsync(AdminInvitationId);
        ShopUser = await _userRepository.GetUserByHttpContext(HttpContext);
        if (AdminInvitation == null || ShopUser == null) return RedirectToPage(UrlProvider.Index);

        return Page();
    }

    public async Task<IActionResult> OnPostRespondToInvitationAsync(RequestActionEnum action)
    {
        await _adminService.RespondToInvitationAsync(AdminInvitationId, action);

        return RedirectToPage(UrlProvider.Index);
    }
}