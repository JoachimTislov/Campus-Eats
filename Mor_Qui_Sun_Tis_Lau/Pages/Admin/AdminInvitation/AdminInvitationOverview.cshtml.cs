using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Admin.AdminInvitation;

[Authorize(Roles = "Admin")]
public class AdminInvitationOverviewModel(IUserRepository userRepository) : PageModel
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    public List<User>? Customers { get; private set; }

    public async Task OnGetAsync()
    {
        await InitCustomers();
    }

    private async Task InitCustomers()
    {
        Customers = await _userRepository.GetCustomers();
    }
}