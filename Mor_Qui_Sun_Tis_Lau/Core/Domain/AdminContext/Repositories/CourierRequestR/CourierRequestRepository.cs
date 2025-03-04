using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Repositories.CourierRequestR;

public class CourierRequestRepository(IDbRepository<CourierRoleRequest> dbRepository) : ICourierRequestRepository
{
    private readonly IDbRepository<CourierRoleRequest> _dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));

    public async Task<CourierRoleRequest> CreateCourierRoleRequest(Guid userId, string resume)
    {
        var courierRoleRequest = new CourierRoleRequest(userId, resume);

        await _dbRepository.AddAsync(courierRoleRequest);

        return courierRoleRequest;
    }

    public async Task<CourierRoleRequest?> GetCourierRequestById(Guid requestId)
    {
        return await _dbRepository.GetByIdAsync(requestId);
    }

    public async Task<CourierRoleRequest?> GetCourierRoleRequestByUserId(Guid userId)
    {
        return await _dbRepository.WhereFirstOrDefaultAsync(i => i.UserId == userId);
    }

    public async Task<List<CourierRoleRequest>> GetPendingCourierRoleRequestsAsync()
    {
        return await _dbRepository.WhereToListAsync(r => r.Status == CourierRoleRequestStatusEnum.Pending);
    }
}

