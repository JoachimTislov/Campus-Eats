using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Handlers;

public class AdminInvitationEventHandler(IUserRepository userRepository) : INotificationHandler<AdminInvitationEvent>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    public async Task Handle(AdminInvitationEvent notification, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(notification.UserId);
        if (user == null) return; // Can't assign role to null

        user.PotentiallyHasAdminAccount = true;
        await _userRepository.UpdateUser(user);
    }
}
