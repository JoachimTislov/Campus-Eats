using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;

public interface IUserService
{
    Task<bool> CheckIfUserIsAssignedRole(HttpContext httpContext, string role);
    Task<bool> CheckIfUserIsAssignedRole(ClaimsPrincipal claimsPrincipal, string role);
    Task<bool> CheckIfUserIsAssignedRole(User? user, string role);
    Task AssignRoleToUserAsync(User user, string Role);
    Task<(bool success, IdentityError[] errors)> Register(string email, string firstName, string lastName, bool shouldSignInUser, string? password = null, UserLoginInfo? userLoginInfo = null);
    Task<(bool Succeeded, string ErrorMessage)> Login(User? user, string password);
    Task<bool> CheckPassword(User? user, string password);
    Task<(bool success, string token)> GeneratePasswordResetTokenAsync(User? user);
    Task<(bool success, bool userNotFound, IdentityError[] errors)> ChangePassword(User? user, string token, string newPassword);
    Task<bool> LoginWithThirdParty(HttpContext httpContext);
}