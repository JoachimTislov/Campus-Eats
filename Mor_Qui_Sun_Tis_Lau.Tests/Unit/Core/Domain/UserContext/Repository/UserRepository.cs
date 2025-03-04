
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.UserContext.Repository;

public class UserRepositoryTests
{
    private readonly Mock<FakeUserManager> _mockUserManager = new();

    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        _userRepository = new(_mockUserManager.Object);
    }

    private readonly static User EmptyUser = new();

    [Fact]
    public async Task CreateUser_ShouldCreateUserWithPassword_WhenPasswordIsNotNull()
    {
        _mockUserManager
            .Setup(m => m.CreateAsync(
                It.IsAny<User>(),
                It.IsAny<string>()
            )).ReturnsAsync(IdentityResult.Success);

        var result = await _userRepository.CreateUser(It.IsAny<User>(), "Password");

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task CreateUser_ShouldCreateUserWithPassword_WhenPasswordIsNull()
    {
        _mockUserManager
            .Setup(m => m.CreateAsync(
                It.IsAny<User>()
            )).ReturnsAsync(IdentityResult.Success);

        var result = await _userRepository.CreateUser(It.IsAny<User>(), null);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task UpdateUserProfile_ShouldReturnNull_WhenEmailIsInvalid()
    {
        User? _user = null;
        _mockUserManager
            .Setup(sm => sm.FindByEmailAsync(
                It.IsAny<string>()
            ))
            .ReturnsAsync(_user);

        var (success, user) = await _userRepository.UpdateUserProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

        Assert.Null(user);
        Assert.False(success);
    }

    [Fact]
    public async Task UpdateUserProfile_ShouldReturnUser_WhenSuccessfullyUpdatedTheUser()
    {
        _mockUserManager
            .Setup(sm => sm.FindByEmailAsync(
                It.IsAny<string>()
            ))
            .ReturnsAsync(EmptyUser);

        var firstName = "firstName";
        var lastName = "lastName";

        var (success, user) = await _userRepository.UpdateUserProfile(It.IsAny<string>(), firstName, lastName, It.IsAny<string>());

        Assert.NotNull(user);
        Assert.True(success);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);

        _mockUserManager.Verify(m => m.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateUser()
    {
        await _userRepository.UpdateUser(EmptyUser);

        _mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByClaimPrincipal_ShouldTryToFindUserByClaimPrincipal()
    {
        var user = await _userRepository.GetUserByClaimsPrincipal(It.IsAny<ClaimsPrincipal>());

        _mockUserManager.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);

        Assert.Null(user);
    }

    [Fact]
    public async Task GetUserByEmail_ShouldTryToFindUserByMail()
    {
        var user = await _userRepository.GetUserByEmail(It.IsAny<string>());

        _mockUserManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Once);

        Assert.Null(user);
    }

    [Fact]
    public async Task GetUserByName_ShouldTryToFindUserByName()
    {
        var user = await _userRepository.GetUserByName(It.IsAny<string>());

        _mockUserManager.Verify(m => m.FindByNameAsync(It.IsAny<string>()), Times.Once);

        Assert.Null(user);
    }

    [Fact]
    public async Task GetUserByHttpContext_ShouldGetUserByEmailFromClaims_WhenAuthenticationTypeIsNotIdentity()
    {
        var httpContext = FakeAuthenticationHandler.CreateHttpContextForThirdPartyTesting();

        var user = await _userRepository.GetUserByHttpContext(httpContext);

        _mockUserManager.Verify(m => m.GetUserAsync(httpContext.User), Times.Never);

        _mockUserManager.Verify(m => m.FindByEmailAsync("test@example.com"), Times.Once);

        Assert.Null(user);
    }

    [Fact]
    public async Task GetUserByHttpContext_ShouldGetUserByClaimsPrincipal_WhenAuthenticationTypeIsIdentity()
    {
        var authenticationType = "Identity.Application";

        var httpContext = FakeAuthenticationHandler.CreateHttpContextForThirdPartyTesting(authenticationType: authenticationType);

        var user = await _userRepository.GetUserByHttpContext(httpContext);

        _mockUserManager.Verify(m => m.GetUserAsync(httpContext.User), Times.Once);

        Assert.Null(user);
    }

    [Fact]
    public async Task GetExternalLoginValues_ShouldReturnDefaultValues_WhenAuthenticationTypeIsNull()
    {
        var (claims, loginProvider, providerKey) = await _userRepository.GetExternalLoginValuesAsync(new DefaultHttpContext());

        Assert.Empty(claims);
        Assert.Equal(string.Empty, loginProvider);
        Assert.Equal(string.Empty, providerKey);
    }

    [Fact]
    public async Task GetExternalLoginValues_ShouldReturnLoginValues_WhenThereAreClaims_LoginProviderAndAProviderKey()
    {
        var httpContext = FakeAuthenticationHandler.CreateHttpContextForThirdPartyTesting();

        var (claims, loginProvider, providerKey) = await _userRepository.GetExternalLoginValuesAsync(httpContext);

        Assert.NotEmpty(claims);
        Assert.Equal("mock", loginProvider);
        Assert.Equal("test-user-id", providerKey);
    }
}