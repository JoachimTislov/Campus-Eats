using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Handlers;

public class CourierRequestEvaluatedHandler(IEmailService userService, INotificationService notificationService) : INotificationHandler<CourierRoleRequestEvaluated>
{
    private readonly IEmailService _emailService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly INotificationService _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

    public async Task Handle(CourierRoleRequestEvaluated notification, CancellationToken cancellationToken)
    {
        var subject = "An admin has evaluated you courier request!";

        var message = notification.Approved
        ? "Congrats, you are now a courier, welcome on board! :)"
        : "We regret to inform you that you weren't qualified for the courier position :(";

        var textBody = $"Hey! \n {message} \n\n -- Campus Eats";

        _ = _emailService.SendMailAsync(notification.UsersEmail, subject, textBody);

        await _notificationService.CreateNotification(notification.UserId, subject, message);

        await _notificationService.CreateAlertNotification(notification.UserId, message, "/Dashboard", "Go to courier dashboard");
        await _notificationService.ReloadClientPage(notification.UserId);
    }
}