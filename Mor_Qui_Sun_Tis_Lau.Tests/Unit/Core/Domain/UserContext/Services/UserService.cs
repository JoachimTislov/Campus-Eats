using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.UserContext.Services;

public class UserServiceTests
{
    private readonly Mock<FakeUserManager> _mockUserManager = new();
    private readonly Mock<FakeSignInManager> _mockSignInManager = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IMediator> _mockMediatR = new();

    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new(_mockUserManager.Object, _mockSignInManager.Object, _mockUserRepository.Object, _mockMediatR.Object);
    }

    private readonly static User EmptyUser = new();

    [Fact]
    public async Task CheckIfUserIsAssignedRoleByClaimsPrincipal_ShouldReturnFalse_WhenUserIsNull()
    {
        User? user = null;
        _mockUserRepository
            .Setup(sm => sm.GetUserByClaimsPrincipal(
                It.IsAny<ClaimsPrincipal>()
            ))
            .ReturnsAsync(user);

        var result = await _userService.CheckIfUserIsAssignedRole(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>());

        Assert.False(result);

        _mockUserManager.Verify(m => m.IsInRoleAsync(EmptyUser, It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CheckIfUserIsAssignedRoleByClaimsPrincipal_ShouldCheckIfUserIsAssignedToRole_WhenUserIsNotNull()
    {
        _mockUserRepository
            .Setup(sm => sm.GetUserByClaimsPrincipal(
                It.IsAny<ClaimsPrincipal>()
            ))
            .ReturnsAsync(EmptyUser);

        var result = await _userService.CheckIfUserIsAssignedRole(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>());

        Assert.False(result);

        _mockUserManager.Verify(m => m.IsInRoleAsync(EmptyUser, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CheckIfUserIsAssignedRoleByUser_ShouldReturnFalse_WhenUserIsNull()
    {
        var result = await _userService.CheckIfUserIsAssignedRole(It.IsAny<User>(), It.IsAny<string>());

        Assert.False(result);

        _mockUserManager.Verify(m => m.IsInRoleAsync(EmptyUser, It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CheckIfUserIsAssignedRole_ShouldCheckIfUserIsAssignedToRole_WhenUserIsNotNull()
    {
        await _userService.CheckIfUserIsAssignedRole(EmptyUser, It.IsAny<string>());

        _mockUserManager.Verify(m => m.IsInRoleAsync(EmptyUser, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task AssignRoleToUserAsync_ShouldAssignRole_WhenTheUserDoesNotHaveTheRole()
    {
        _mockUserManager
            .Setup(sm => sm.IsInRoleAsync(
                It.IsAny<User>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(false);

        await _userService.AssignRoleToUserAsync(It.IsAny<User>(), It.IsAny<string>());

        _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task AssignRoleToUserAsync_ShouldNotAssignRole_WhenTheUserHasTheRole()
    {
        _mockUserManager
            .Setup(sm => sm.IsInRoleAsync(
                It.IsAny<User>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(true);

        await _userService.AssignRoleToUserAsync(It.IsAny<User>(), It.IsAny<string>());

        _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Register_ShouldReturnFalseAndErrors_WhenCreateUserFails()
    {
        var errors = new IdentityError[]
        {
            new() { Description = "Creation of user failed" },
        };

        _mockUserRepository
            .Setup(sm => sm.CreateUser(
                It.IsAny<User>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(IdentityResult.Failed(errors));

        var (success, registerErrors) = await _userService.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), "Test@Password");

        Assert.False(success);
        Assert.Equal(errors, registerErrors);
    }

    [Fact]
    public async Task Register_ShouldReturnTrue_WhenCreateUserSucceedsWithPasswordAndUserShouldNotBeSignedIn()
    {
        var result = IdentityResult.Success;

        _mockUserRepository
            .Setup(sm => sm.CreateUser(
                It.IsAny<User>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(IdentityResult.Success);

        var (success, registerErrors) = await _userService.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), false, "Test@Password", UserLoginInfo);

        VerifyActionsAfterSuccessfullyCreatingUser();

        _mockSignInManager.Verify(m => m.SignInAsync(It.IsAny<User>(), false, null), Times.Never);

        Assert.True(success);
        Assert.Empty(registerErrors);
    }

    private readonly UserLoginInfo UserLoginInfo = new(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

    private void VerifyActionsAfterSuccessfullyCreatingUser()
    {
        _mockMediatR.Verify(m => m.Publish(It.Is<UserRegistered>(e => e.UsersEmail == It.IsAny<string>()), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), "Customer"), Times.Once);
        _mockUserManager.Verify(m => m.AddLoginAsync(It.IsAny<User>(), UserLoginInfo), Times.Once);
    }

    [Fact]
    public async Task Register_ShouldReturnTrue_WhenCreateUserSucceedsWithoutPasswordAndUserShouldBeSignedIn()
    {
        _mockUserRepository
            .Setup(sm => sm.CreateUser(
                It.IsAny<User>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(IdentityResult.Success);

        var (success, registerErrors) = await _userService.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true, userLoginInfo: UserLoginInfo);

        VerifyActionsAfterSuccessfullyCreatingUser();

        _mockSignInManager.Verify(m => m.SignInAsync(It.IsAny<User>(), false, null), Times.Once);

        Assert.True(success);
        Assert.Empty(registerErrors);
    }

    [Fact]
    public async Task Login_ShouldReturnFalseAndError_WhenUserIsNull()
    {
        var (Succeeded, ErrorMessage) = await _userService.Login(null, It.IsAny<string>());

        Assert.False(Succeeded);
        Assert.Equal("Invalid login", ErrorMessage);
    }

    [Fact]
    public async Task Login_ShouldReturnFalseAndError_WhenPasswordIsIncorrect()
    {
        _mockSignInManager
            .Setup(sm => sm.PasswordSignInAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()
            ))
            .ReturnsAsync(SignInResult.Failed);

        var (Succeeded, ErrorMessage) = await _userService.Login(EmptyUser, It.IsAny<string>());

        Assert.False(Succeeded);
        Assert.Equal("Wrong password", ErrorMessage);
    }

    [Fact]
    public async Task Login_ShouldReturnTrueAndEmptyErrorMessage_WhenLoginIsSuccessful()
    {
        _mockSignInManager
            .Setup(sm => sm.PasswordSignInAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()
            ))
            .ReturnsAsync(SignInResult.Success);

        var (Succeeded, ErrorMessage) = await _userService.Login(EmptyUser, It.IsAny<string>());

        _mockSignInManager.Verify(m => m.PasswordSignInAsync(EmptyUser, It.IsAny<string>(), It.IsAny<bool>(), false), Times.Once);
        _mockUserRepository.Verify(m => m.UpdateUser(EmptyUser), Times.Once);

        Assert.True(Succeeded);
        Assert.Equal(string.Empty, ErrorMessage);

        Assert.NotNull(EmptyUser.LastLoginDate);
    }

    [Fact]
    public async Task GeneratePasswordResetTokenAsync_ShouldReturnFalse_WhenUserIsNull()
    {
        var (success, token) = await _userService.GeneratePasswordResetTokenAsync(null);

        Assert.False(success);
        Assert.Equal(string.Empty, token);
    }

    [Fact]
    public async Task GeneratePasswordResetTokenAsync_ShouldReturnTrue_WhenTokenGenerationIsSuccessful()
    {
        var token = "token";
        _mockUserManager
            .Setup(sm => sm.GeneratePasswordResetTokenAsync(
                It.IsAny<User>()
            ))
            .ReturnsAsync(token);

        var (success, generatedToken) = await _userService.GeneratePasswordResetTokenAsync(EmptyUser);

        _mockUserManager.Verify(m => m.GeneratePasswordResetTokenAsync(EmptyUser), Times.Once);

        Assert.True(success);
        Assert.Equal(token, generatedToken);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnFalse_Errors_AndUserNotFoundIsTrue_WhenUserIsNull()
    {
        var (_success, userNotFound, errors) = await _userService.ChangePassword(null, It.IsAny<string>(), It.IsAny<string>());

        Assert.False(_success);
        Assert.True(userNotFound);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnFalseAndErrors_WhenChangePasswordFails()
    {
        var errors = new IdentityError[]
        {
            new() { Description = "Change password failed" },
        };
        var result = IdentityResult.Failed(errors);

        _mockUserManager
            .Setup(sm => sm.ResetPasswordAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(result);

        var (success, userNotFound, changePasswordErrors) = await _userService.ChangePassword(EmptyUser, It.IsAny<string>(), It.IsAny<string>());

        Assert.False(success);
        Assert.False(userNotFound);
        Assert.Equal(errors, changePasswordErrors);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnTrue_WhenPasswordIsChangedSuccessfully()
    {
        _mockUserManager
            .Setup(sm => sm.ResetPasswordAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(IdentityResult.Success);

        var (success, userNotFound, errors) = await _userService.ChangePassword(EmptyUser, It.IsAny<string>(), It.IsAny<string>());

        Assert.True(success);
        Assert.False(userNotFound);
        Assert.Empty(errors);

        Assert.True(EmptyUser.HasChangedPassword);

        _mockUserRepository.Verify(m => m.UpdateUser(EmptyUser), Times.Once);
    }

    [Fact]
    public async Task LoginInWithThirdParty_ShouldReturnFalse_WhenProviderKeyIsNullOrWhitespace()
    {
        _mockUserRepository
            .Setup(m => m.GetExternalLoginValuesAsync(
                It.IsAny<HttpContext>()
            )).ReturnsAsync((It.IsAny<List<Claim>>(), It.IsAny<string>(), string.Empty));

        var result = await _userService.LoginWithThirdParty(It.IsAny<HttpContext>());

        Assert.False(result);

        _mockUserRepository.Verify(m => m.GetExternalLoginValuesAsync(It.IsAny<HttpContext>()), Times.Once);
    }

    [Fact]
    public async Task LoginInWithThirdParty_ShouldReturnTrue_WhenExternalLoginSucceeds()
    {
        _mockUserRepository
            .Setup(m => m.GetExternalLoginValuesAsync(
                It.IsAny<HttpContext>()
            )).ReturnsAsync((It.IsAny<List<Claim>>(), It.IsAny<string>(), "providerKey"));

        _mockSignInManager
            .Setup(m => m.ExternalLoginSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            ))
            .ReturnsAsync(SignInResult.Success);

        var result = await _userService.LoginWithThirdParty(It.IsAny<HttpContext>());

        Assert.True(result);

        _mockUserRepository.Verify(m => m.GetExternalLoginValuesAsync(It.IsAny<HttpContext>()), Times.Once);
        _mockSignInManager.Verify(m => m.ExternalLoginSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task LoginInWithThirdParty_ShouldReturnFalse_WhenEmailIsNullOrWhitespace()
    {
        _mockUserRepository
            .Setup(m => m.GetExternalLoginValuesAsync(
                It.IsAny<HttpContext>()
            )).ReturnsAsync((It.IsAny<List<Claim>>(), It.IsAny<string>(), "providerKey"));

        _mockSignInManager
            .Setup(m => m.ExternalLoginSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            ))
            .ReturnsAsync(SignInResult.Failed);

        _mockUserRepository
            .Setup(m => m.GetEmailByClaims(
                It.IsAny<List<Claim>>()
            )).Returns(string.Empty);

        var result = await _userService.LoginWithThirdParty(It.IsAny<HttpContext>());

        Assert.False(result);

        _mockUserRepository.Verify(m => m.GetExternalLoginValuesAsync(It.IsAny<HttpContext>()), Times.Once);
        _mockSignInManager.Verify(m => m.ExternalLoginSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        _mockUserRepository.Verify(m => m.GetEmailByClaims(It.IsAny<List<Claim>>()), Times.Once);
    }

    [Fact]
    public async Task LoginInWithThirdParty_ShouldReturnTrue_WhenUserIsFoundByEmailAndIsSignedIn()
    {
        _mockUserRepository
            .Setup(m => m.GetExternalLoginValuesAsync(
                It.IsAny<HttpContext>()
            )).ReturnsAsync((It.IsAny<List<Claim>>(), It.IsAny<string>(), "providerKey"));

        _mockSignInManager
            .Setup(m => m.ExternalLoginSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            ))
            .ReturnsAsync(SignInResult.Failed);

        var email = "test@email.com";

        _mockUserRepository
            .Setup(m => m.GetEmailByClaims(
                It.IsAny<List<Claim>>()
            )).Returns(email);

        _mockUserRepository
            .Setup(m => m.GetUserByEmail(
                It.IsAny<string>()
            )).ReturnsAsync(EmptyUser);


        var result = await _userService.LoginWithThirdParty(It.IsAny<HttpContext>());

        _mockUserRepository.Verify(m => m.GetExternalLoginValuesAsync(It.IsAny<HttpContext>()), Times.Once);
        _mockSignInManager.Verify(m => m.ExternalLoginSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        _mockUserRepository.Verify(m => m.GetEmailByClaims(It.IsAny<List<Claim>>()), Times.Once);
        _mockUserRepository.Verify(m => m.GetUserByEmail(email), Times.Once);
        _mockSignInManager.Verify(m => m.SignInAsync(EmptyUser, false, It.IsAny<string>()), Times.Once);

        Assert.True(result);
    }

    [Fact]
    public async Task LoginInWithThirdParty_ShouldReturnTrue_WhenUserHasBeenCreatedSuccessfully()
    {
        _mockUserRepository
            .Setup(m => m.GetExternalLoginValuesAsync(
                It.IsAny<HttpContext>()
            )).ReturnsAsync((It.IsAny<List<Claim>>(), It.IsAny<string>(), "providerKey"));

        _mockSignInManager
            .Setup(m => m.ExternalLoginSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            ))
            .ReturnsAsync(SignInResult.Failed);

        var email = "test@email.com";

        _mockUserRepository
            .Setup(m => m.GetEmailByClaims(
                It.IsAny<List<Claim>>()
            )).Returns(email);

        _mockUserRepository
            .Setup(m => m.GetUserByEmail(
                It.IsAny<string>()
            )).ReturnsAsync((User?)null);

        _mockUserRepository
            .Setup(m => m.GetClaimTypeValue(
                ClaimTypes.GivenName,
                It.IsAny<List<Claim>>()
            )).Returns(It.IsAny<string>());

        _mockUserRepository
            .Setup(m => m.GetClaimTypeValue(
                ClaimTypes.Surname,
                It.IsAny<List<Claim>>()
            )).Returns(It.IsAny<string>());

        _mockUserRepository
           .Setup(sm => sm.CreateUser(
               It.IsAny<User>(),
               It.IsAny<string>()
           ))
           .ReturnsAsync(IdentityResult.Success);

        var role = "Customer";
        _mockUserManager
            .Setup(m => m.IsInRoleAsync(It.IsAny<User>(), role)).ReturnsAsync(false);

        var result = await _userService.LoginWithThirdParty(It.IsAny<HttpContext>());

        _mockUserRepository.Verify(m => m.GetExternalLoginValuesAsync(It.IsAny<HttpContext>()), Times.Once);
        _mockSignInManager.Verify(m => m.ExternalLoginSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        _mockUserRepository.Verify(m => m.GetEmailByClaims(It.IsAny<List<Claim>>()), Times.Once);
        _mockUserRepository.Verify(m => m.GetUserByEmail(email), Times.Once);
        _mockUserRepository.Verify(m => m.GetClaimTypeValue(ClaimTypes.GivenName, It.IsAny<List<Claim>>()), Times.Once);
        _mockUserRepository.Verify(m => m.GetClaimTypeValue(ClaimTypes.Surname, It.IsAny<List<Claim>>()), Times.Once);
        _mockUserRepository.Verify(m => m.CreateUser(It.IsAny<User>(), It.IsAny<string>()), Times.Once);

        // Happens after successfully created a user
        _mockMediatR.Verify(m => m.Publish(It.Is<UserRegistered>(e => e.UsersEmail == email), It.IsAny<CancellationToken>()), Times.Once);

        _mockUserManager.Verify(m => m.IsInRoleAsync(It.IsAny<User>(), role), Times.Once);
        _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), role), Times.Once);

        _mockSignInManager.Verify(m => m.SignInAsync(It.IsAny<User>(), false, It.IsAny<string>()), Times.Once);
        _mockUserManager.Verify(m => m.AddLoginAsync(It.IsAny<User>(), It.IsAny<UserLoginInfo>()), Times.Once);

        Assert.True(result);
    }
}


