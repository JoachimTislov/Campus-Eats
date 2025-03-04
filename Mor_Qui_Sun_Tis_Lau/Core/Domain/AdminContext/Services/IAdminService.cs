using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;

public interface IAdminService
{
    Task CreateAdminAccount(string adminName);
    Task<User?> GetAdminByName(string name);
    Task<(bool succeeded, string errorMessage)> Login(string adminName, string password);
    Task<(bool success, string error)> GeneratePasswordResetTokenAsync(string name);
    Task<CourierRoleRequest> CreateCourierRoleRequest(Guid userId, string resume);
    Task<CourierRoleRequest?> GetCourierRoleRequestByUserId(Guid userId);
    Task<List<CourierRoleRequest>> GetCourierRolePendingRequestsAsync();
    Task RespondToCourierRequest(Guid userId, Guid requestId, RequestActionEnum action);
    Task SendAdminInvitation(Guid userId, string resume);
    Task<AdminInvitation?> GetAdminInvitationByIdAsync(Guid invitationId);
    Task RespondToInvitationAsync(Guid invitationId, RequestActionEnum response);
    decimal GetDeliveryFee();
    void SetDeliveryFee(decimal deliveryFee);
}