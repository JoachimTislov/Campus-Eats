using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.UserContext.Handlers;

public class CourierRoleRequestEvaluatedHandlerTests
{
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly CourierRoleRequestEvaluatedHandler _handler;

    public CourierRoleRequestEvaluatedHandlerTests()
    {
        _handler = new(_mockUserService.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenEmailIsInvalid_ShouldNotAssignRole()
    {
        var notification = new CourierRoleRequestEvaluated(It.IsAny<Guid>(), false, It.IsAny<string>());

        User? user = null;
        _mockUserRepository.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);

        await _handler.Handle(notification, It.IsAny<CancellationToken>());

        _mockUserService
            .Verify(x => x.AssignRoleToUserAsync(It.IsAny<User>(), "Courier"), Times.Never, "Role was not assigned correctly.");
    }

    [Fact]
    public async Task Handle_WhenRoleIsApproved_ShouldAssignRoleToUser()
    {
        var notification = new CourierRoleRequestEvaluated(It.IsAny<Guid>(), true, It.IsAny<string>());

        User user = new();
        _mockUserRepository.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);

        await _handler.Handle(notification, It.IsAny<CancellationToken>());

        _mockUserService
            .Verify(x => x.AssignRoleToUserAsync(user, "Courier"), Times.Once, "Role was not assigned correctly.");
    }

    [Fact]
    public async Task Handle_WhenRoleIsNotApproved_ShouldNotAssignRoleToUser()
    {
        var notification = new CourierRoleRequestEvaluated(It.IsAny<Guid>(), false, It.IsAny<string>());

        User user = new();
        _mockUserRepository.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);

        await _handler.Handle(notification, It.IsAny<CancellationToken>());

        _mockUserService
            .Verify(x => x.AssignRoleToUserAsync(user, "Courier"), Times.Never, "Role was not assigned correctly.");
    }
}