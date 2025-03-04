using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Handlers;

public class UserAcceptedAdminInvitationHandler(IEmailService emailService, INotificationService notificationService) : INotificationHandler<UserAcceptedAdminInvitation>
{
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly INotificationService _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

    public async Task Handle(UserAcceptedAdminInvitation notification, CancellationToken cancellationToken)
    {
        var emailBody = $"Hello, here is your new admin account:\nUserName: {notification.AdminName} \nPassword: Test1234*";
        _ = _emailService.SendMailAsync(notification.UsersEmail, "New admin account", emailBody);

        var message = "Successfully created an admin account for you to use, check your email!";

        await _notificationService.CreateAlertNotification(notification.UserId, message);
    }
}