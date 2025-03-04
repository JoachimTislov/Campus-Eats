using Microsoft.AspNetCore.SignalR;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Hub;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.NotificationContext.Services.NotificationS;

public class NotificationServiceTests
{
    private readonly Mock<IHubContext<NotificationHub, INotificationClient>> _mockHubContext = new();
    private readonly Mock<INotificationRepository> _mockNotificationRepository = new();

    private readonly NotificationService _notificationService;
    private readonly Guid UserId = new();

    public NotificationServiceTests()
    {
        _notificationService = new(_mockHubContext.Object, _mockNotificationRepository.Object);

        Mock<IHubCallerClients<INotificationClient>> _mockHubClients = new();
        Mock<INotificationClient> _mockNotificationClient = new();

        _mockHubContext.Setup(m => m.Clients).Returns(_mockHubClients.Object);
        _mockHubClients.Setup(c => c.User(UserId.ToString())).Returns(_mockNotificationClient.Object);
    }

    [Fact]
    public async Task CreateNotification()
    {
        await _notificationService.CreateNotification(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

        _mockNotificationRepository.Verify(m => m.CreateNotification(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetNotificationById()
    {
        await _notificationService.GetNotificationById(It.IsAny<Guid>());

        _mockNotificationRepository.Verify(m => m.GetNotificationById(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetNotificationsFilteredByUserId()
    {
        await _notificationService.GetNotificationsFilteredByUserId(It.IsAny<Guid>());

        _mockNotificationRepository.Verify(m => m.GetNotificationsFilteredByUserId(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task NotifyUser()
    {
        await _notificationService.NotifyUser(UserId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

        _mockHubContext.Verify(m => m.Clients.User(UserId.ToString()).NotifyClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ReloadClientPage()
    {
        await _notificationService.ReloadClientPage(UserId);

        _mockHubContext.Verify(m => m.Clients.User(UserId.ToString()).ReloadPage(), Times.Once);
    }

    [Fact]
    public void DeliveryStatusUpdate()
    {

    }

    [Fact]
    public void NewOrderPlaced()
    {

    }
}