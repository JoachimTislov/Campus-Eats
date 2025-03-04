using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Repositories.CourierRequestR;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.AdminContext.Repositories.CourierRequestR;

public class CourierRequestRepositoryTests
{
    private readonly Mock<IDbRepository<CourierRoleRequest>> _mockDbRepository = new();

    private readonly CourierRequestRepository _courierRequestRepository;

    public CourierRequestRepositoryTests()
    {
        _courierRequestRepository = new(_mockDbRepository.Object);
    }

    [Fact]
    public async Task CreateCourierRoleRequest()
    {
        var userId = Guid.NewGuid();
        var resume = "resume";

        var courierRoleRequest = await _courierRequestRepository.CreateCourierRoleRequest(userId, resume);

        _mockDbRepository.Verify(m => m.AddAsync(courierRoleRequest), Times.Once);

        Assert.Equal(userId, courierRoleRequest.UserId);
        Assert.Equal(resume, courierRoleRequest.Resume);
    }

    [Fact]
    public async Task GetCourierRequestById()
    {
        var courierRoleRequestId = Guid.NewGuid();

        await _courierRequestRepository.GetCourierRequestById(courierRoleRequestId);

        _mockDbRepository.Verify(m => m.GetByIdAsync(courierRoleRequestId), Times.Once);
    }

    [Fact]
    public async Task GetCourierRoleRequestByUserId()
    {
        var userId = Guid.NewGuid();

        await _courierRequestRepository.GetCourierRoleRequestByUserId(userId);

        _mockDbRepository.Verify(m => m.WhereFirstOrDefaultAsync(i => i.UserId == userId), Times.Once);
    }

    [Fact]
    public async Task GetPendingCourierRoleRequestsAsync()
    {
        await _courierRequestRepository.GetPendingCourierRoleRequestsAsync();

        _mockDbRepository.Verify(m => m.WhereToListAsync(r => r.Status == CourierRoleRequestStatusEnum.Pending), Times.Once);
    }
}

