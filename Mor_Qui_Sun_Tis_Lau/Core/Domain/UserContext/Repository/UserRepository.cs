using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<IdentityResult> CreateUser(User user, string? password)
    {
        return password is not null
        ? await _userManager.CreateAsync(user, password)
        : await _userManager.CreateAsync(user);
    }

    public async Task UpdateUser(User user)
    {
        await _userManager.UpdateAsync(user);
    }

    public async Task<(bool, User?)> UpdateUserProfile(string email, string firstName, string lastName, string phoneNumber)
    {
        var user = await GetUserByEmail(email);
        if (user == null) return (false, null);

        user.Email = email;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.PhoneNumber = phoneNumber;

        await UpdateUser(user);

        return (true, user);
    }

    public async Task UpdateUserAddress(string email, string addressLine, string city, string postalCode)
    {
        var user = await GetUserByEmail(email);
        if (user == null) return;

        user.Address.AddressLine = addressLine;
        user.Address.City = city;
        user.Address.PostalCode = postalCode;

        await UpdateUser(user);
    }

    public async Task<List<User>> GetUsersByRole(string role)
    {
        return [.. (await _userManager.GetUsersInRoleAsync(role))];
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    public async Task<User?> GetUserByClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
    {
        return await _userManager.GetUserAsync(claimsPrincipal);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<User?> GetUserByName(string name)
    {
        return await _userManager.FindByNameAsync(name);
    }

    public async Task<User?> GetUserByHttpContext(HttpContext httpContext)
    {
        if (httpContext.User.Identity?.AuthenticationType == "Identity.Application")
        {
            return await GetUserByClaimsPrincipal(httpContext.User);
        }

        var claims = await GetClaims(httpContext);
        if (claims == null) return null;

        var email = GetEmailByClaims(claims);
        return string.IsNullOrWhiteSpace(email) ? null : await GetUserByEmail(email);
    }

    private static async Task<List<Claim>?> GetClaims(HttpContext httpContext)
    {
        var authenticateResult = await httpContext.AuthenticateAsync(httpContext.User.Identity!.AuthenticationType);
        return authenticateResult.Principal?.Claims.ToList();
    }

    public string GetEmailByClaims(List<Claim> claims) => GetClaimTypeValue(ClaimTypes.Email, claims);

    public string GetClaimTypeValue(string claimType, List<Claim> claims) => claims.FirstOrDefault(x => x.Type == claimType)?.Value ?? string.Empty;

    public async Task<(List<Claim> claims, string loginProvider, string providerKey)> GetExternalLoginValuesAsync(HttpContext httpContext)
    {
        var authenticateType = httpContext.User.Identity?.AuthenticationType;
        if (authenticateType == null) return ([], string.Empty, string.Empty);

        var claims = await GetClaims(httpContext);
        if (claims is null) return ([], string.Empty, string.Empty);

        var providerKey = GetClaimTypeValue(ClaimTypes.NameIdentifier, claims);

        return (claims, authenticateType, providerKey);
    }

    public async Task<List<User>> GetCustomers()
    {
        var users = _userManager.Users.ToList();
        var customers = new List<User>();
        foreach (var user in users)
        {
            if (await _userManager.IsInRoleAsync(user, "Customer") && !user.PotentiallyHasAdminAccount)
            {
                customers.Add(user);
            }
        }

        return customers;
    }
}