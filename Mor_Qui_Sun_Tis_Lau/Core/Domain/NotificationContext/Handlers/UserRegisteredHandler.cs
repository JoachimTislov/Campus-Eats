using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Handlers;

public class UserRegisteredHandler(IEmailService userService, INotificationService notificationService) : INotificationHandler<UserRegistered>
{
    private readonly IEmailService _emailService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly INotificationService _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

    public async Task Handle(UserRegistered notification, CancellationToken cancellationToken)
    {
        if (notification.SendMail)
        {
            var subject = "Welcome to campus eats!";
            var textBody = "Hey, \n Welcome to campus eats! \n\n -- Campus Eats";

            _ = _emailService.SendMailAsync(notification.UsersEmail, subject, textBody);
        }

        await _notificationService.CreateAlertNotification(notification.UserId, "Welcome to campus eats, enjoy the application!");
        await _notificationService.ReloadClientPage(notification.UserId);
    }
}