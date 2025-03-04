
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Repository;

public class NotificationRepository(IDbRepository<Notification> dbNotificationRepository, IDbRepository<AlertNotification> dbAlertNotificationRepository) : INotificationRepository
{
    private readonly IDbRepository<Notification> _dbNotificationRepository = dbNotificationRepository ?? throw new ArgumentNullException(nameof(dbNotificationRepository));
    private readonly IDbRepository<AlertNotification> _dbAlertNotificationRepository = dbAlertNotificationRepository ?? throw new ArgumentNullException(nameof(dbAlertNotificationRepository));

    public async Task CreateAlertNotification(Guid userId, string message, string? link, string? nameOfLink)
    {
        var alertNotification = new AlertNotification(userId, message, link, nameOfLink);

        await _dbAlertNotificationRepository.AddAsync(alertNotification);
    }

    public async Task<List<AlertNotification>> GetAlertNotificationsByUserId(Guid userId)
    {
        var notifications = await _dbAlertNotificationRepository.WhereToListAsync(e => e.UserId == userId);

        await _dbAlertNotificationRepository.RemoveRange(notifications);

        return notifications;
    }

    public async Task CreateNotification(Guid userId, string title, string description, string? link)
    {
        var notification = new Notification(userId, title, description, link);

        await _dbNotificationRepository.AddAsync(notification);
    }

    public async Task<Notification?> GetNotificationById(Guid notificationId)
    {
        return await _dbNotificationRepository.GetByIdAsync(notificationId);
    }

    public async Task<List<Notification>> GetNotificationsFilteredByUserId(Guid userId)
    {
        return await _dbNotificationRepository.WhereOrderByDescendingToListAsync(n => n.UserId == userId, n => n.CreatedAt);
    }
}