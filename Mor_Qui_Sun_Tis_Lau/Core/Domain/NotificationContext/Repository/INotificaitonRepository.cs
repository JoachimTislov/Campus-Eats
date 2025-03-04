
namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Repository;

public interface INotificationRepository
{
    Task CreateAlertNotification(Guid userId, string message, string? link, string? nameOfLink);
    Task<List<AlertNotification>> GetAlertNotificationsByUserId(Guid userId);
    Task CreateNotification(Guid userId, string title, string description, string? link);
    Task<Notification?> GetNotificationById(Guid notificationId);
    Task<List<Notification>> GetNotificationsFilteredByUserId(Guid userId);
}