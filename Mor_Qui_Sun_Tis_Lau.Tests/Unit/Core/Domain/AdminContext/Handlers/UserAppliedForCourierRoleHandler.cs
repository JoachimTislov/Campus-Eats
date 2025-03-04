using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.AdminContext.Handlers;

public class UserAppliedForCourierRoleHandlerTests
{
    private readonly Mock<IAdminService> _mockAdminService = new();

    private readonly UserAppliedForCourierRoleHandler _userAppliedForCourierRoleHandler;

    public UserAppliedForCourierRoleHandlerTests()
    {
        _userAppliedForCourierRoleHandler = new(_mockAdminService.Object);
    }

    [Fact]
    public async Task CourierAppliedToRole_HandlerShouldCreateNewRequest()
    {
        var userId = Guid.NewGuid();
        var resume = "resume";

        var userAppliedForCourierRole = new UserAppliedForCourierRole(userId, resume);

        await _userAppliedForCourierRoleHandler.Handle(userAppliedForCourierRole, It.IsAny<CancellationToken>());

        _mockAdminService.Verify(m => m.CreateCourierRoleRequest(userId, resume), Times.Once);
    }
}