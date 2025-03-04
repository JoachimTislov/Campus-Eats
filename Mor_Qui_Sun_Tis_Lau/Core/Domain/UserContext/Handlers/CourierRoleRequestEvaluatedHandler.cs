using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Handlers;

public class CourierRoleRequestEvaluatedHandler(IUserService userService, IUserRepository userRepository) : INotificationHandler<CourierRoleRequestEvaluated>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    public async Task Handle(CourierRoleRequestEvaluated notification, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmail(notification.UsersEmail);
        if (user == null) return; // Can't assign role to null

        if (notification.Approved)
        {
            await _userService.AssignRoleToUserAsync(user, "Courier");
        }
    }
}
