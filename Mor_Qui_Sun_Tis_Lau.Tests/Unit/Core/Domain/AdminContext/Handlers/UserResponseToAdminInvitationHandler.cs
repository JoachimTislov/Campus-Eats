
using MediatR;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.AdminContext.Handlers;

public class UserResponseToAdminInvitationHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IAdminService> _mockAdminService = new();
    private readonly Mock<IMediator> _mockMediator = new();

    private readonly UserResponseToAdminInvitationHandler _userResponseToAdminInvitationHandler;

    public UserResponseToAdminInvitationHandlerTests()
    {
        _userResponseToAdminInvitationHandler = new(_mockUserRepository.Object, _mockAdminService.Object, _mockMediator.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturn_WhenUserIsNull()
    {
        var userId = Guid.NewGuid();
        var UserResponseToAdminInvitation = new UserResponseToAdminInvitation(It.IsAny<RequestActionEnum>(), userId, It.IsAny<Guid>());

        User? user = null;
        _mockUserRepository
            .Setup(m => m.GetUserById(userId))
            .ReturnsAsync(user);

        await _userResponseToAdminInvitationHandler.Handle(UserResponseToAdminInvitation, CancellationToken.None);

        _mockUserRepository.Verify(m => m.GetUserById(userId), Times.Once);

        _mockAdminService.Verify(m => m.CreateAdminAccount(It.IsAny<string>()), Times.Never);
        _mockMediator.Verify(m => m.Publish(It.IsAny<UserAcceptedAdminInvitation>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturn_WhenUserDoesNotHaveAnEmail()
    {
        var userId = Guid.NewGuid();
        var UserResponseToAdminInvitation = new UserResponseToAdminInvitation(It.IsAny<RequestActionEnum>(), userId, It.IsAny<Guid>());

        User user = new();
        _mockUserRepository
            .Setup(m => m.GetUserById(userId))
            .ReturnsAsync(user);

        await _userResponseToAdminInvitationHandler.Handle(UserResponseToAdminInvitation, CancellationToken.None);

        _mockUserRepository.Verify(m => m.GetUserById(userId), Times.Once);
        _mockAdminService.Verify(m => m.CreateAdminAccount(It.IsAny<string>()), Times.Never);
        _mockMediator.Verify(m => m.Publish(It.IsAny<UserAcceptedAdminInvitation>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotCreateAdminAndPublishEvent_WhenUserRejectsInvitation()
    {
        var userId = Guid.NewGuid();
        var UserResponseToAdminInvitation = new UserResponseToAdminInvitation(RequestActionEnum.Reject, userId, It.IsAny<Guid>());

        User user = new("firstName", "lastName", "email");
        _mockUserRepository
            .Setup(m => m.GetUserById(userId))
            .ReturnsAsync(user);

        Assert.NotNull(user.Email);

        await _userResponseToAdminInvitationHandler.Handle(UserResponseToAdminInvitation, CancellationToken.None);

        _mockUserRepository.Verify(m => m.GetUserById(userId), Times.Once);
        _mockAdminService.Verify(m => m.CreateAdminAccount(It.IsAny<string>()), Times.Never);
        _mockMediator.Verify(m => m.Publish(It.IsAny<UserAcceptedAdminInvitation>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateAdminAndPublishEvent_WhenUserAcceptsInvitation()
    {
        var userId = Guid.NewGuid();
        var UserResponseToAdminInvitation = new UserResponseToAdminInvitation(RequestActionEnum.Accept, userId, It.IsAny<Guid>());

        User user = new("firstName", "lastName", "email");
        _mockUserRepository
            .Setup(m => m.GetUserById(userId))
            .ReturnsAsync(user);

        User? null_user = null;
        _mockUserRepository
            .Setup(m => m.GetUserByName(It.IsAny<string>()))
            .ReturnsAsync(null_user);

        Assert.NotNull(user.Email);

        await _userResponseToAdminInvitationHandler.Handle(UserResponseToAdminInvitation, CancellationToken.None);

        _mockUserRepository.Verify(m => m.GetUserById(userId), Times.Once);
        _mockAdminService.Verify(m => m.CreateAdminAccount(It.IsAny<string>()), Times.Once);
        _mockMediator.Verify(m => m.Publish(It.IsAny<UserAcceptedAdminInvitation>(), CancellationToken.None), Times.Once);
    }
}