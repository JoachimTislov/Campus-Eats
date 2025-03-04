using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;

public class UserService(UserManager<User> userManager, SignInManager<User> signInManager, IUserRepository userRepository, IMediator mediator) : IUserService
{
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly SignInManager<User> _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task<bool> CheckIfUserIsAssignedRole(HttpContext httpContext, string role)
    {
        var user = await _userRepository.GetUserByHttpContext(httpContext);
        if (user == null) return false;

        return await IsInRoleAsync(user, role);
    }

    public async Task<bool> CheckIfUserIsAssignedRole(ClaimsPrincipal claimsPrincipal, string role)
    {
        var user = await _userRepository.GetUserByClaimsPrincipal(claimsPrincipal);
        if (user == null) return false;

        return await IsInRoleAsync(user, role);
    }

    public async Task<bool> CheckIfUserIsAssignedRole(User? user, string role)
    {
        if (user == null) return false;

        return await IsInRoleAsync(user, role);
    }

    private async Task<bool> IsInRoleAsync(User user, string role) => await _userManager.IsInRoleAsync(user, role);

    public async Task AssignRoleToUserAsync(User user, string role)
    {
        if (!await IsInRoleAsync(user, role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }
    }

    public async Task<(bool success, IdentityError[] errors)> Register(string email, string firstName, string lastName, bool shouldSignInUser, string? password = null, UserLoginInfo? userLoginInfo = null)
    {
        var user = new User(firstName, lastName, email);
        var createUserResult = await _userRepository.CreateUser(user, password);

        if (createUserResult.Succeeded)
        {
            await _mediator.Publish(new UserRegistered(user.Id, firstName, email, shouldSignInUser));
            await AssignRoleToUserAsync(user, "Customer");

            // Check here to prevent issues when seeding users
            if (shouldSignInUser) await _signInManager.SignInAsync(user, isPersistent: false);

            if (userLoginInfo is not null) await _userManager.AddLoginAsync(user, userLoginInfo);

            return (true, Array.Empty<IdentityError>());
        }
        else
        {
            return (false, createUserResult.Errors.ToArray());
        }
    }

    private async Task UpdateUser(User user) => await _userRepository.UpdateUser(user);

    public async Task<(bool Succeeded, string ErrorMessage)> Login(User? user, string password)
    {
        if (user != null)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

            user.LastLoginDate = DateTime.UtcNow;
            await UpdateUser(user);

            return signInResult.Succeeded ? (true, string.Empty) : (false, "Wrong password");
        }

        return (false, "Invalid login");
    }

    public async Task<bool> CheckPassword(User? user, string password)
    {
        if (user == null) return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<(bool success, string token)> GeneratePasswordResetTokenAsync(User? user)
    {
        if (user == null) return (false, string.Empty);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return (true, token);
    }

    public async Task<(bool success, bool userNotFound, IdentityError[] errors)> ChangePassword(User? user, string token, string newPassword)
    {
        if (user == null) return (false, true, Array.Empty<IdentityError>());

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (result.Succeeded)
        {
            // Used for admin
            user.HasChangedPassword = true;
            await UpdateUser(user);

            return (true, false, Array.Empty<IdentityError>());
        }
        else
        {
            return (false, false, result.Errors.ToArray());
        }
    }

    public async Task<bool> LoginWithThirdParty(HttpContext httpContext)
    {
        var (claims, loginProvider, providerKey) = await _userRepository.GetExternalLoginValuesAsync(httpContext);
        if (string.IsNullOrWhiteSpace(providerKey)) return false;

        // Attempting to login with external login provider
        if ((await _signInManager.ExternalLoginSignInAsync(
            loginProvider,
            providerKey,
            isPersistent: false)
        ).Succeeded) return true;

        var email = _userRepository.GetEmailByClaims(claims);
        if (string.IsNullOrWhiteSpace(email)) return false;

        // Attempting to login with already created user account
        var user = await _userRepository.GetUserByEmail(email);
        if (user != null)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return true;
        }

        // Creating a new account for the user
        return await CreateExternalUserAsync(email, claims, loginProvider, providerKey);
    }

    private async Task<bool> CreateExternalUserAsync(string email, List<Claim> claims, string loginProvider, string providerKey)
    {
        var firstName = _userRepository.GetClaimTypeValue(ClaimTypes.GivenName, claims);
        var lastName = _userRepository.GetClaimTypeValue(ClaimTypes.Surname, claims);

        var (success, _) = await Register(email, firstName, lastName, true, userLoginInfo: new UserLoginInfo(loginProvider, providerKey, loginProvider));

        return success;
    }
}