using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Handlers;

public class AdminInvitationHandler(IEmailService emailService, IUserRepository userRepository, INotificationService notificationService) : INotificationHandler<AdminInvitationEvent>
{
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly INotificationService _notificationService = notificationService ?? throw new ArgumentNullException(nameof(userRepository));

    public async Task Handle(AdminInvitationEvent notification, CancellationToken cancellationToken)
    {
        var toUser = await _userRepository.GetUserById(notification.UserId);
        if (toUser == null || toUser.Email == null) return;

        var subject = "You have been requested to be an admin";
        var mailBody = "If you want to become an admin login in the application and accept the request in the notifications page";
        var notificationBody = "You have been requested to be an admin, please respond this notification to inform administrators if you want to join";

        _ = _emailService.SendMailAsync(toUser.Email, subject, mailBody);
        await _notificationService.CreateNotification(toUser.Id, "Admin Invitation", notificationBody, $"/Respond-to-admin-invitation/{notification.AdminInvitationId}");

        await _notificationService.CreateAlertNotification(notification.UserId, "You have gotten invitation to become an admin, view your notifications to answer it");
        await _notificationService.ReloadClientPage(notification.UserId);
    }
}