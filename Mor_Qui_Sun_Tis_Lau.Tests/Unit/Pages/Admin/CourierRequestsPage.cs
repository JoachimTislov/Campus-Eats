using Microsoft.AspNetCore.Mvc;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Admin;

public class CourierRequestsPageTests
{
    private readonly Mock<IAdminService> _mockAdminService = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();

    private readonly CourierRequestsModel _courierRequestsModel;

    public CourierRequestsPageTests()
    {
        _courierRequestsModel = new(_mockAdminService.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task OnGetAsync_ShouldExecuteLoadRequestData()
    {
        var user = new User();

        var courierRoleRequest = new CourierRoleRequest(user.Id, "resume");
        List<CourierRoleRequest> courierRoleRequests = [courierRoleRequest];

        _mockAdminService
            .Setup(m => m.GetCourierRolePendingRequestsAsync())
            .ReturnsAsync(courierRoleRequests);

        _mockUserRepository
            .Setup(m => m.GetUserById(user.Id))
            .ReturnsAsync(user);

        await _courierRequestsModel.OnGetAsync();

        Assert.NotEmpty(_courierRequestsModel.PendingRequests);
        Assert.NotEmpty(_courierRequestsModel.RequestUserMapping);
    }

    [Fact]
    public async Task OnPosUpdateAsync()
    {
        var userId = Guid.NewGuid();
        var requestId = Guid.NewGuid();
        var action = RequestActionEnum.Accept;

        var result = await _courierRequestsModel.OnPostUpdateAsync(userId, requestId, action);

        Assert.IsType<RedirectToPageResult>(result);

        _mockAdminService.Verify(m => m.RespondToCourierRequest(userId, requestId, action), Times.Once);
    }
}

