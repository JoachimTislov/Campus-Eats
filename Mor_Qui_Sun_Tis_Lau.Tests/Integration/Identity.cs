using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Integration;

public class IdentityTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly UserManager<User> _UserManager;
    private readonly SignInManager<User> _SignInManager;
    private readonly HttpContext _HttpContext;
    private readonly string Email = "Test@Email";
    private readonly User User;

    public IdentityTests(CustomWebApplicationFactory<Program> fixture)
    {
        _UserManager = fixture.Services.GetRequiredService<UserManager<User>>();
        _SignInManager = fixture.Services.GetRequiredService<SignInManager<User>>();
        _HttpContext = fixture.Services.GetRequiredService<IHttpContextAccessor>().HttpContext!;

        User = new User("FirstName", "LastName", Email);
    }

    [Fact]
    public async Task CreateUser_SignInWithOutPassword_CheckSignInStatus_SimulateSignOut_ChangePassword_AttemptToSignInWithOldPassword_SignInWithNewPasswordAnd_DeleteUser()
    {
        var _user = new User("FirstName", "LastName", Email);
        var oldPassword = "TestPassword@123";

        var createUser = await _UserManager.CreateAsync(_user, oldPassword);
        Assert.True(createUser.Succeeded);

        await _SignInManager.SignInAsync(_user, isPersistent: false);

        var IsSignedIn = _SignInManager.IsSignedIn(_HttpContext.User);
        Assert.True(IsSignedIn);
        Assert.True(_HttpContext.User.Identity!.IsAuthenticated);

        // A redirect to another page is needed to get a new httpContext
        // await _SignInManager.SignOutAsync();
        // Remove cookies: _HttpContext.Response.Cookies["CookieName"].Expires = DateTime.Now.AddYears(-1);
        // Assert.False(_HttpContext.User.Identity!.IsAuthenticated);

        // Simulating signing out without redirecting -- https://stackoverflow.com/questions/4050925/page-user-identity-isauthenticated-still-true-after-formsauthentication-signout
        _HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
        var user = await _UserManager.FindByEmailAsync(Email);

        Assert.NotNull(user);

        string token = await _UserManager.GeneratePasswordResetTokenAsync(user);

        // Alerting password
        var newPassword = "NewTestPassword@123";
        await _UserManager.ResetPasswordAsync(user, token, newPassword);

        var loginAttemptWithOldPasswordAttempt = await _SignInManager.PasswordSignInAsync(user, oldPassword, isPersistent: false, lockoutOnFailure: false);
        Assert.False(loginAttemptWithOldPasswordAttempt.Succeeded);
        Assert.False(_HttpContext.User.Identity!.IsAuthenticated);

        var loginAttemptWithNewPasswordAttempt = await _SignInManager.PasswordSignInAsync(user, newPassword, isPersistent: false, lockoutOnFailure: false);
        Assert.True(loginAttemptWithNewPasswordAttempt.Succeeded);
        Assert.True(_HttpContext.User.Identity!.IsAuthenticated);

        var deleteUser = await _UserManager.DeleteAsync(user);
        Assert.True(deleteUser.Succeeded);
    }

    [Theory]
    [InlineData("CorrectPassword@123", true, null)]
    [InlineData("NoDigitPassword@", false, "PasswordRequiresDigit")]
    [InlineData("short1@", false, "PasswordTooShort")]
    [InlineData("noupper1@", false, "PasswordRequiresUpper")]
    [InlineData("NOLOWER1@", false, "PasswordRequiresLower")]
    [InlineData("NoSpecialChar123", false, "PasswordRequiresNonAlphanumeric")]
    public async Task CheckIfPasswordValidationWorksCorrectly(string password, bool expectedResult, string? expectedError)
    {
        var createUser = await _UserManager.CreateAsync(User, password);
        Assert.Equal(expectedResult, createUser.Succeeded);

        if (expectedError != null && !expectedResult)
        {
            Assert.Contains(createUser.Errors, error => error.Code == expectedError);
        }
        else
        {
            // Deleting user with valid password
            var deleteUser = await _UserManager.DeleteAsync(User);
            Assert.True(deleteUser.Succeeded);
        }
    }
}