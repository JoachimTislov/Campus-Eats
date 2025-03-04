using MediatR;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Repositories.AdminInvitationR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Repositories.CourierRequestR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.AdminContext.Services;

public class AdminServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<IMediator> _mockMediatR = new();
    private readonly Mock<IAdminInvitationRepository> _mockAdminInvitationRepository = new();
    private readonly Mock<ICourierRequestRepository> _mockCourierRequestsRepository = new();
    private readonly AdminService _adminService;
    private readonly static User EmptyAdmin = new();

    public AdminServiceTests()
    {
        _adminService = new(_mockCourierRequestsRepository.Object, _mockAdminInvitationRepository.Object, _mockUserRepository.Object, _mockUserService.Object, _mockMediatR.Object);
    }

    [Fact]
    public async Task CreateAdminAccount()
    {
        var adminName = "adminName";

        await _adminService.CreateAdminAccount(adminName);

        _mockUserRepository.Verify(m => m.CreateUser(It.Is<User>(p => p.UserName == adminName), "Test1234*"), Times.Once);
        _mockUserService.Verify(m => m.AssignRoleToUserAsync(It.Is<User>(p => p.UserName == adminName), "Admin"), Times.Once);
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ShouldReturnInvalidLoginAttempt()
    {
        User? admin = null;
        _mockUserRepository
            .Setup(sm => sm.GetUserByName(
                It.IsAny<string>()
            ))
            .ReturnsAsync(admin);

        var (succeeded, errorMessage) = await _adminService.Login(It.IsAny<string>(), It.IsAny<string>());

        Assert.False(succeeded);
        Assert.Equal("Invalid login attempt", errorMessage);
    }

    [Fact]
    public async Task Login_WithoutAdminRole_ShouldReturnYouAreNotAnAdmin()
    {
        _mockUserRepository
            .Setup(sm => sm.GetUserByName(
                It.IsAny<string>()
            ))
            .ReturnsAsync(EmptyAdmin);

        _mockUserService
            .Setup(sm => sm.CheckIfUserIsAssignedRole(
                It.IsAny<User>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(false);

        var (succeeded, errorMessage) = await _adminService.Login(It.IsAny<string>(), It.IsAny<string>());

        Assert.False(succeeded);
        Assert.Equal("You are not an admin", errorMessage);
    }

    [Fact]
    public async Task Login_WhenAdminHasNotChangedPassword_ShouldReturnChangePassword()
    {
        _mockUserRepository
            .Setup(sm => sm.GetUserByName(
                It.IsAny<string>()
            ))
            .ReturnsAsync(EmptyAdmin);

        _mockUserService
            .Setup(sm => sm.CheckIfUserIsAssignedRole(
                It.IsAny<User>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(true);

        _mockUserService
            .Setup(sm => sm.CheckPassword(
                It.IsAny<User>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(true);

        EmptyAdmin.HasChangedPassword = false;

        var (succeeded, errorMessage) = await _adminService.Login(It.IsAny<string>(), It.IsAny<string>());

        Assert.False(succeeded);
        Assert.Equal("Change Password", errorMessage);
    }

    [Fact]
    public async Task Login_WithCorrectValues_ShouldReturnTrue()
    {
        EmptyAdmin.HasChangedPassword = true;

        _mockUserRepository
            .Setup(sm => sm.GetUserByName(
                It.IsAny<string>()
            ))
            .ReturnsAsync(EmptyAdmin);

        _mockUserService
            .Setup(sm => sm.CheckIfUserIsAssignedRole(
                It.IsAny<User>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(true);

        _mockUserService
            .Setup(sm => sm.Login(
                It.IsAny<User>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync((true, string.Empty));

        var (succeeded, errorMessage) = await _adminService.Login(It.IsAny<string>(), It.IsAny<string>());

        Assert.True(succeeded);
        Assert.Equal(string.Empty, errorMessage);
    }

    [Fact]
    public async Task GeneratePasswordResetTokenAsync()
    {
        var adminName = "adminName";

        Admin admin = new(adminName);
        _mockUserRepository
            .Setup(m => m.GetUserByName(adminName))
            .ReturnsAsync(admin);

        await _adminService.GeneratePasswordResetTokenAsync(adminName);

        _mockUserService.Verify(m => m.GeneratePasswordResetTokenAsync(It.Is<User>(p => p.UserName == adminName)), Times.Once);
    }

    [Fact]
    public async Task CreateCourierRoleRequest()
    {
        var userId = Guid.NewGuid();
        var resume = "resume";

        await _adminService.CreateCourierRoleRequest(userId, resume);

        _mockCourierRequestsRepository.Verify(m => m.CreateCourierRoleRequest(userId, resume), Times.Once);
    }

    [Fact]
    public async Task GetCourierRoleRequestByUserId()
    {
        var userId = Guid.NewGuid();

        await _adminService.GetCourierRoleRequestByUserId(userId);

        _mockCourierRequestsRepository.Verify(m => m.GetCourierRoleRequestByUserId(userId), Times.Once);
    }

    [Fact]
    public async Task RespondToCourierRequest_ShouldPublishEvent_WhenUserIsNotNullAndHasAnEmail()
    {
        var requestId = Guid.NewGuid();

        User user = new("firstName", "lastName", "testUser@email.com");
        _mockUserRepository
            .Setup(m => m.GetUserById(user.Id))
            .ReturnsAsync(user);

        _mockCourierRequestsRepository
            .Setup(m => m.GetCourierRequestById(requestId))
            .ReturnsAsync(new CourierRoleRequest(user.Id, "resume"));

        await _adminService.RespondToCourierRequest(user.Id, requestId, RequestActionEnum.Accept);

        _mockUserRepository.Verify(m => m.GetUserById(user.Id), Times.Once);
        _mockCourierRequestsRepository.Verify(m => m.GetCourierRequestById(requestId), Times.Once);
        _mockMediatR.Verify(m => m.Publish(It.Is<CourierRoleRequestEvaluated>(c => c.UserId == user.Id && c.Approved == true && c.UsersEmail == user.Email), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RespondToCourierRequest_ShouldNotPublishEvent_WhenUserIsNull()
    {
        await _adminService.RespondToCourierRequest(It.IsAny<Guid>(), It.IsAny<Guid>(), RequestActionEnum.Accept);

        _mockUserRepository.Verify(m => m.GetUserById(It.IsAny<Guid>()), Times.Once);
        _mockCourierRequestsRepository.Verify(m => m.GetCourierRequestById(It.IsAny<Guid>()), Times.Never);
        _mockMediatR.Verify(m => m.Publish(It.IsAny<CourierRoleRequestEvaluated>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task RespondToCourierRequest_ShouldNotPublishEvent_WhenUserDoNotHaveAnEmail()
    {
        User user = new();
        _mockUserRepository
            .Setup(m => m.GetUserById(user.Id))
            .ReturnsAsync(user);

        await _adminService.RespondToCourierRequest(It.IsAny<Guid>(), It.IsAny<Guid>(), RequestActionEnum.Accept);

        _mockUserRepository.Verify(m => m.GetUserById(It.IsAny<Guid>()), Times.Once);
        _mockCourierRequestsRepository.Verify(m => m.GetCourierRequestById(It.IsAny<Guid>()), Times.Never);
        _mockMediatR.Verify(m => m.Publish(It.IsAny<CourierRoleRequestEvaluated>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task RespondToCourierRequest_ShouldNotPublishEvent_WhenRequestIsNull()
    {
        User user = new("firstName", "lastName", "testUser@email.com");
        _mockUserRepository
            .Setup(m => m.GetUserById(user.Id))
            .ReturnsAsync(user);

        CourierRoleRequest? courierRoleRequest = null;
        _mockCourierRequestsRepository
            .Setup(m => m.GetCourierRequestById(It.IsAny<Guid>()))
            .ReturnsAsync(courierRoleRequest);

        await _adminService.RespondToCourierRequest(It.IsAny<Guid>(), It.IsAny<Guid>(), RequestActionEnum.Accept);

        _mockUserRepository.Verify(m => m.GetUserById(It.IsAny<Guid>()), Times.Once);
        _mockCourierRequestsRepository.Verify(m => m.GetCourierRequestById(It.IsAny<Guid>()), Times.Once);
        _mockMediatR.Verify(m => m.Publish(It.IsAny<CourierRoleRequestEvaluated>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task GetCourierRolePendingRequestsAsync()
    {
        await _adminService.GetCourierRolePendingRequestsAsync();

        _mockCourierRequestsRepository.Verify(m => m.GetPendingCourierRoleRequestsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAdminInvitationByIdAsync()
    {
        var adminInvitationId = Guid.NewGuid();

        await _adminService.GetAdminInvitationByIdAsync(adminInvitationId);

        _mockAdminInvitationRepository.Verify(m => m.GetAdminInvitationByIdAsync(adminInvitationId), Times.Once);
    }

    [Fact]
    public async Task RespondToInvitationAsync_ShouldPublishEvent_WhenInvitationIsNotNull()
    {
        var action = RequestActionEnum.Accept;

        AdminInvitation adminInvitation = new(Guid.NewGuid(), "resume");
        _mockAdminInvitationRepository
            .Setup(m => m.GetAdminInvitationByIdAsync(adminInvitation.Id))
            .ReturnsAsync(adminInvitation);

        await _adminService.RespondToInvitationAsync(adminInvitation.Id, action);

        _mockMediatR.Verify(m => m.Publish(It.Is<UserResponseToAdminInvitation>(u => u.Action == action && u.UserId == adminInvitation.UserId && u.InvitationId == adminInvitation.Id), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RespondToInvitationAsync_ShouldNotPublishEvent_WhenInvitationIsNull()
    {
        AdminInvitation? adminInvitation = null;
        _mockAdminInvitationRepository
            .Setup(m => m.GetAdminInvitationByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(adminInvitation);

        await _adminService.RespondToInvitationAsync(It.IsAny<Guid>(), It.IsAny<RequestActionEnum>());

        _mockMediatR.Verify(m => m.Publish(It.IsAny<UserResponseToAdminInvitation>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public void GetDeliveryFee_ShouldReturn50ByDefault()
    {
        var deliveryFee = _adminService.GetDeliveryFee();

        Assert.Equal(50, deliveryFee);
    }

    [Fact]
    public void ChangeDeliveryFee_ShouldChangeTheValueOnJsonFile()
    {
        _adminService.SetDeliveryFee(100);

        var deliveryFee = _adminService.GetDeliveryFee();

        Assert.Equal(100, deliveryFee);

        _adminService.SetDeliveryFee(50); // Maintain default value
    }
}