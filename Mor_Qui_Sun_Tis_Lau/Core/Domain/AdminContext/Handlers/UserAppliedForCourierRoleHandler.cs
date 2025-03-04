using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Handlers;

public class UserAppliedForCourierRoleHandler(IAdminService adminService) : INotificationHandler<UserAppliedForCourierRole>
{
    private readonly IAdminService _adminService = adminService ?? throw new ArgumentException(nameof(adminService));

    public async Task Handle(UserAppliedForCourierRole notification, CancellationToken cancellationToken)
    {
        await _adminService.CreateCourierRoleRequest(notification.UserId, notification.Resume);
    }
}
