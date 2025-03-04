
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.NotificationContext.Handlers;

public class UserAcceptedAdminInvitationHandlerTests
{
    private readonly Mock<IEmailService> _mockEmailService = new();
    private readonly Mock<INotificationService> _mockNotificationService = new();

    private readonly UserAcceptedAdminInvitationHandler _userAcceptedAdminInvitationHandler;

    public UserAcceptedAdminInvitationHandlerTests()
    {
        _userAcceptedAdminInvitationHandler = new(_mockEmailService.Object, _mockNotificationService.Object);
    }

    [Fact]
    public async Task Handle_ShouldSendMailAndNotifyUser()
    {
        var userId = Guid.NewGuid();
        var email = "testUser@Email.com";
        var adminName = "adminName";

        var userAcceptedAdminInvitation = new UserAcceptedAdminInvitation(userId, email, adminName);

        await _userAcceptedAdminInvitationHandler.Handle(userAcceptedAdminInvitation, CancellationToken.None);

        _mockEmailService
            .Verify(m => m.SendMailAsync(
                email,
                "New admin account",
                $"Hello, here is your new admin account:\nUserName: {adminName} \nPassword: Test1234*"
            ), Times.Once);

        _mockNotificationService
            .Verify(m => m.CreateAlertNotification(
                userId,
                "Successfully created an admin account for you to use, check your email!",
                It.IsAny<string>(),
                It.IsAny<string>()
            ));
    }
}