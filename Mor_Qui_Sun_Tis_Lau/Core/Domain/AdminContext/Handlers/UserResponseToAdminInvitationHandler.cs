using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Handlers;

public class UserResponseToAdminInvitationHandler(IUserRepository userRepository, IAdminService adminService, IMediator mediator) : INotificationHandler<UserResponseToAdminInvitation>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IAdminService _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task Handle(UserResponseToAdminInvitation notification, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(notification.UserId);
        if (user == null || user.Email == null) return;

        if (notification.Action == RequestActionEnum.Accept)
        {
            var uniqueAdminName = await CreateUniqueAdminName(user.FirstName);

            await _adminService.CreateAdminAccount(uniqueAdminName);

            await _mediator.Publish(new UserAcceptedAdminInvitation(user.Id, user.Email, uniqueAdminName), cancellationToken);
        }
        else
        {
            user.PotentiallyHasAdminAccount = false;
            await _userRepository.UpdateUser(user);
        }
    }

    private async Task<string> CreateUniqueAdminName(string firstName)
    {
        // Create a new admin account and notify via email
        var adminNameTag = $"{firstName}-Admin";
        var adminName = adminNameTag;

        // Checking that the admin name does not exist
        bool valid = false;
        while (!valid)
        {
            User? existingUser = await _userRepository.GetUserByName(adminName);
            if (existingUser == null)
            {
                valid = true;
            }
            else
            {
                int randomNumber = new Random().Next(100, 1000);
                adminName = adminNameTag + randomNumber;
            }
        }
        return adminName;
    }
}