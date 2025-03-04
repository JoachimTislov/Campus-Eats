using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Notifications;

[Authorize(Roles = "Customer")]
public class NotificationModel(IUserRepository userRepository, INotificationService notificationService) : PageModel
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly INotificationService _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

    public List<Notification> Notifications { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        return await LoadNotificationDataAsync();
    }

    private async Task<IActionResult> LoadNotificationDataAsync()
    {
        var user = await _userRepository.GetUserByClaimsPrincipal(User);
        if (user == null) return RedirectToPage(UrlProvider.Index);

        Notifications = await _notificationService.GetNotificationsFilteredByUserId(user.Id);

        return Page();
    }
}