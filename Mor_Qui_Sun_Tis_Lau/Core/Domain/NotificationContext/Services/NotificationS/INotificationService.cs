
namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;

public interface INotificationService
{
    Task SendQueuedNotifications(Guid userId);
    Task CreateAlertNotification(Guid userId, string message, string? link = null, string? nameOfLink = null);
    Task CreateNotification(Guid userId, string title, string description, string? link = null);
    Task<Notification?> GetNotificationById(Guid notificationId);
    Task<List<Notification>> GetNotificationsFilteredByUserId(Guid userId);
    Task NotifyUser(Guid userId, string message, string? link, string? nameOfLink);
    Task ReloadClientPage(Guid userId);
}