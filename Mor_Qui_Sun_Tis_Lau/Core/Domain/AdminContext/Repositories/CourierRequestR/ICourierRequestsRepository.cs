using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Repositories.CourierRequestR;

public interface ICourierRequestRepository
{
    Task<CourierRoleRequest> CreateCourierRoleRequest(Guid userId, string resume);
    Task<CourierRoleRequest?> GetCourierRequestById(Guid requestId);
    Task<CourierRoleRequest?> GetCourierRoleRequestByUserId(Guid userId);
    Task<List<CourierRoleRequest>> GetPendingCourierRoleRequestsAsync();
}