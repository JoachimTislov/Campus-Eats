using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Admin;

[Authorize(Roles = "Admin")]
public class CourierRequestsModel(IAdminService adminService, IUserRepository userRepository) : PageModel
{
    private readonly IAdminService _adminService = adminService;
    private readonly IUserRepository _userRepository = userRepository;

    public List<CourierRoleRequest> PendingRequests { get; set; } = [];
    public Dictionary<CourierRoleRequest, User> RequestUserMapping = [];

    public async Task OnGetAsync()
    {
        await LoadRequestData();
    }

    private async Task LoadRequestData()
    {
        PendingRequests = [.. await _adminService.GetCourierRolePendingRequestsAsync()];

        foreach (var request in PendingRequests)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user != null)
            {
                RequestUserMapping.Add(request, user);
            }
        }
    }

    public async Task<IActionResult> OnPostUpdateAsync(Guid userId, Guid requestId, RequestActionEnum action)
    {
        await _adminService.RespondToCourierRequest(userId, requestId, action);

        return RedirectToPage();
    }
}