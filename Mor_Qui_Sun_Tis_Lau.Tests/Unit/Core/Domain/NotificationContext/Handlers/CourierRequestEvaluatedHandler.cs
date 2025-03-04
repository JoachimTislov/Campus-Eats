
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.NotificationContext.Handlers;

public class CourierRequestEvaluatedHandlerTests
{
    private readonly Mock<IEmailService> _mockEmailService = new();
    private readonly Mock<INotificationService> _mockNotificationService = new();

    private readonly CourierRequestEvaluatedHandler _courierRequestEvaluatedHandler;

    public CourierRequestEvaluatedHandlerTests()
    {
        _courierRequestEvaluatedHandler = new(_mockEmailService.Object, _mockNotificationService.Object);
    }

    [Theory]
    [InlineData("Congrats, you are now a courier, welcome on board! :)", true)]
    [InlineData("We regret to inform you that you weren't qualified for the courier position :(", false)]
    public async Task Handle_ShouldSendMailNotifyUserAndCreateNotificationWithCorrespondingMessage(string correspondingMessage, bool approved)
    {
        Guid UserId = Guid.NewGuid();
        string Email = "testUser@Email.com";
        string Subject = "An admin has evaluated you courier request!";

        var courierRoleRequestEvaluated = new CourierRoleRequestEvaluated(UserId, approved, Email);

        var message = $"Hey! \n {correspondingMessage} \n\n -- Campus Eats";

        await _courierRequestEvaluatedHandler.Handle(courierRoleRequestEvaluated, CancellationToken.None);

        _mockEmailService
            .Verify(m => m.SendMailAsync(
                Email,
                Subject,
                message
            ), Times.Once);

        _mockNotificationService
            .Verify(m => m.CreateNotification(
                UserId,
                Subject,
                correspondingMessage,
                null
            ), Times.Once);

        _mockNotificationService
            .Verify(m => m.CreateAlertNotification(
                UserId,
                correspondingMessage,
                "/Dashboard",
                "Go to courier dashboard"
            ));

        _mockNotificationService
            .Verify(m => m.ReloadClientPage(
                UserId
            ));
    }
}