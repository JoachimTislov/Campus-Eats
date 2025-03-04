using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

public interface IUserRepository
{
    Task<IdentityResult> CreateUser(User user, string? password);
    Task UpdateUser(User user);
    Task<(bool, User?)> UpdateUserProfile(string email, string firstName, string lastName, string phoneNumber);
    Task UpdateUserAddress(string email, string addressLine, string city, string postalCode);
    Task<List<User>> GetUsersByRole(string role);
    Task<User?> GetUserByClaimsPrincipal(ClaimsPrincipal User);
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserByName(string name);
    Task<User?> GetUserByHttpContext(HttpContext httpContext);
    string GetEmailByClaims(List<Claim> claims);
    string GetClaimTypeValue(string claimType, List<Claim> claims);
    Task<(List<Claim> claims, string loginProvider, string providerKey)> GetExternalLoginValuesAsync(HttpContext httpContext);
    Task<User?> GetUserById(Guid id);
    Task<List<User>> GetCustomers();
}