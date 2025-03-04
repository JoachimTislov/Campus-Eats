using Microsoft.AspNetCore.SignalR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Hub;

public class NotificationHub(INotificationService notificationService) : Hub<INotificationClient>
{
    private readonly INotificationService _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
        {
            await _notificationService.SendQueuedNotifications(Guid.Parse(userId));
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}