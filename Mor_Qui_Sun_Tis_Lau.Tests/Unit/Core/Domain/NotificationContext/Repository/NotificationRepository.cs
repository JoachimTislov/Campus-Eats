
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.NotificationContext.Repository;

public class NotificationRepositoryTests
{
    private readonly Mock<IDbRepository<Notification>> _mockNotificationDbRepository = new();
    private readonly Mock<IDbRepository<AlertNotification>> _mockAlertNotificationDbRepository = new();

    private readonly NotificationRepository _notificationRepository;

    public NotificationRepositoryTests()
    {
        _notificationRepository = new(_mockNotificationDbRepository.Object, _mockAlertNotificationDbRepository.Object);
    }

    [Fact]
    public async Task CreateNotification()
    {
        await _notificationRepository.CreateNotification(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

        _mockNotificationDbRepository.Verify(m => m.AddAsync(It.IsAny<Notification>()), Times.Once);
    }

    [Fact]
    public async Task GetNotificationById()
    {
        await _notificationRepository.GetNotificationById(It.IsAny<Guid>());

        _mockNotificationDbRepository.Verify(m => m.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetNotificationsFilteredByUserId()
    {
        var userId = Guid.NewGuid();

        await _notificationRepository.GetNotificationsFilteredByUserId(userId);

        _mockNotificationDbRepository.Verify(m => m.WhereOrderByDescendingToListAsync(n => n.UserId == userId, n => n.CreatedAt), Times.Once);
    }
}