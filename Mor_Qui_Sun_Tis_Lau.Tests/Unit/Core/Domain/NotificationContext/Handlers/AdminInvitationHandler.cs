
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.NotificationContext.Handlers;

public class AdminInvitationHandlerTests
{
    private readonly Mock<IEmailService> _mockEmailService = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<INotificationService> _mockNotificationService = new();

    private readonly AdminInvitationHandler _adminInvitationHandler;

    public AdminInvitationHandlerTests()
    {
        _adminInvitationHandler = new(_mockEmailService.Object, _mockUserRepository.Object, _mockNotificationService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturn_WhenUserIsNull()
    {
        User? user = null;
        _mockUserRepository
            .Setup(m => m.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(user);

        var adminInvitationEvent = new AdminInvitationEvent(Guid.NewGuid(), It.IsAny<Guid>());

        await _adminInvitationHandler.Handle(adminInvitationEvent, CancellationToken.None);

        _mockEmailService.Verify(m => m.SendMailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockNotificationService.Verify(m => m.CreateNotification(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturn_WhenUsersEmailIsNull()
    {
        User? user = new()
        {
            Email = null
        };
        _mockUserRepository
            .Setup(m => m.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(user);

        var adminInvitationEvent = new AdminInvitationEvent(user.Id, It.IsAny<Guid>());

        await _adminInvitationHandler.Handle(adminInvitationEvent, CancellationToken.None);

        _mockEmailService.Verify(m => m.SendMailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockNotificationService.Verify(m => m.CreateNotification(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }


    [Fact]
    public async Task Handle_ShouldSendMailAndCreateNotification_WhenUserInfoIsValid()
    {
        var email = "testUse@Email.com";
        var user = new User(It.IsAny<string>(), It.IsAny<string>(), email);

        _mockUserRepository
            .Setup(m => m.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(user);

        var adminInvitationId = Guid.NewGuid();
        var adminInvitationEvent = new AdminInvitationEvent(user.Id, adminInvitationId);

        await _adminInvitationHandler.Handle(adminInvitationEvent, CancellationToken.None);

        _mockEmailService
            .Verify(m => m.SendMailAsync(
                email,
                "You have been requested to be an admin",
                "If you want to become an admin login in the application and accept the request in the notifications page"
            ), Times.Once);

        _mockNotificationService
            .Verify(m => m.CreateNotification(
                user.Id,
                "Admin Invitation",
                "You have been requested to be an admin, please respond this notification to inform administrators if you want to join",
                 $"/Respond-to-admin-invitation/{adminInvitationId}"
            ), Times.Once);

        _mockNotificationService
            .Verify(m => m.CreateAlertNotification(
                user.Id,
                "You have gotten invitation to become an admin, view your notifications to answer it",
                It.IsAny<string>(),
                It.IsAny<string>()
            ), Times.Once);

        _mockNotificationService
            .Verify(m => m.ReloadClientPage(
                user.Id
            ));
    }
}
