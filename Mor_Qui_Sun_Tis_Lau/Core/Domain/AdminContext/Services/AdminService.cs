using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Managers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Repositories.AdminInvitationR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Repositories.CourierRequestR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;

public class AdminService(ICourierRequestRepository courierRequestsRepository, IAdminInvitationRepository adminInvitationRepository, IUserRepository userRepository, IUserService userService, IMediator mediator) : IAdminService
{
    private readonly ICourierRequestRepository _courierRequestsRepository = courierRequestsRepository ?? throw new ArgumentNullException(nameof(courierRequestsRepository));
    private readonly IAdminInvitationRepository _adminInvitationRepository = adminInvitationRepository ?? throw new ArgumentNullException(nameof(adminInvitationRepository));
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task CreateAdminAccount(string adminName)
    {
        var adminAccount = new Admin(adminName);

        await _userRepository.CreateUser(adminAccount, "Test1234*");
        await _userService.AssignRoleToUserAsync(adminAccount, "Admin");
    }

    public async Task<User?> GetAdminByName(string adminName) => await _userRepository.GetUserByName(adminName);

    public async Task<(bool succeeded, string errorMessage)> Login(string adminName, string password)
    {
        var admin = await GetAdminByName(adminName);
        if (admin == null) return (false, "Invalid login attempt");

        if (!await _userService.CheckIfUserIsAssignedRole(admin, "Admin"))
        {
            return (false, "You are not an admin");
        }

        if (await _userService.CheckPassword(admin, password) && !admin.HasChangedPassword)
        {
            return (false, "Change Password");
        }

        return await _userService.Login(admin, password);
    }

    public async Task<(bool success, string error)> GeneratePasswordResetTokenAsync(string name)
    {
        return await _userService.GeneratePasswordResetTokenAsync(await GetAdminByName(name));
    }

    public async Task<CourierRoleRequest> CreateCourierRoleRequest(Guid userId, string resume)
    {
        return await _courierRequestsRepository.CreateCourierRoleRequest(userId, resume);
    }

    public async Task<CourierRoleRequest?> GetCourierRoleRequestByUserId(Guid userId)
    {
        return await _courierRequestsRepository.GetCourierRoleRequestByUserId(userId);
    }

    public async Task RespondToCourierRequest(Guid userId, Guid requestId, RequestActionEnum action)
    {
        var user = await _userRepository.GetUserById(userId);
        if (user == null || user.Email == null) return;

        var request = await _courierRequestsRepository.GetCourierRequestById(requestId);
        if (request == null) return;

        request.Approved();

        await _mediator.Publish(new CourierRoleRequestEvaluated(user.Id, action == RequestActionEnum.Accept, user.Email));
    }

    public async Task<List<CourierRoleRequest>> GetCourierRolePendingRequestsAsync()
    {
        return await _courierRequestsRepository.GetPendingCourierRoleRequestsAsync();
    }

    public async Task SendAdminInvitation(Guid userId, string resume)
    {
        await _adminInvitationRepository.SendAdminInvitation(userId, resume);
    }

    public async Task<AdminInvitation?> GetAdminInvitationByIdAsync(Guid invitationId)
    {
        return await _adminInvitationRepository.GetAdminInvitationByIdAsync(invitationId);
    }

    public async Task RespondToInvitationAsync(Guid invitationId, RequestActionEnum action)
    {
        var invitation = await _adminInvitationRepository.GetAdminInvitationByIdAsync(invitationId);
        if (invitation == null) return;

        await _mediator.Publish(new UserResponseToAdminInvitation(action, invitation.UserId, invitation.Id));
    }

    private static readonly ConfigManager _configManger = new("config.json");

    public decimal GetDeliveryFee()
    {
        return _configManger.GetConfig().DeliveryFee;
    }

    public void SetDeliveryFee(decimal deliveryFee)
    {
        var config = _configManger.GetConfig();
        config.DeliveryFee = deliveryFee;
        _configManger.UpdateConfig(config);
    }
}