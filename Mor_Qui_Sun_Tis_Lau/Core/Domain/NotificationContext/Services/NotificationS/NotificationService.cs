using Microsoft.AspNetCore.SignalR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Hub;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;

public class NotificationService(IHubContext<NotificationHub, INotificationClient> hubContext, INotificationRepository notificationRepository) : INotificationService
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    private readonly INotificationRepository _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));

    public async Task SendQueuedNotifications(Guid userId)
    {
        var alertNotifications = await _notificationRepository.GetAlertNotificationsByUserId(userId);
        foreach (var notification in alertNotifications)
        {
            await NotifyUser(userId, notification.Message, notification.Link, notification.NameOfLink);
        }
    }

    public async Task CreateAlertNotification(Guid userId, string message, string? link = null, string? nameOfLink = null)
    {
        await _notificationRepository.CreateAlertNotification(userId, message, link, nameOfLink);
    }

    public async Task CreateNotification(Guid userId, string title, string description, string? link = null)
    {
        await _notificationRepository.CreateNotification(userId, title, description, link);
    }

    public async Task<Notification?> GetNotificationById(Guid notificationId)
    {
        return await _notificationRepository.GetNotificationById(notificationId);
    }

    public async Task<List<Notification>> GetNotificationsFilteredByUserId(Guid userId)
    {
        return await _notificationRepository.GetNotificationsFilteredByUserId(userId);
    }

    public async Task NotifyUser(Guid userId, string message, string? link, string? nameOfLink)
    {
        await _hubContext.Clients.User(userId.ToString()).NotifyClient(message, link, nameOfLink);
    }

    public async Task ReloadClientPage(Guid userId)
    {
        await _hubContext.Clients.User(userId.ToString()).ReloadPage();
    }
}