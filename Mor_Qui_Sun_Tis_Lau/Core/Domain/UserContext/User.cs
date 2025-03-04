using Microsoft.AspNetCore.Identity;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

public class User : IdentityUser<Guid>
{
    public User() { }

    public User(string firstName, string lastName, string email)
    {
        // Assign a string guid to prevent username validation error in identity
        UserName = Guid.NewGuid().ToString(); // Could also assign email
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public Address Address { get; set; } = new();

    public bool PotentiallyHasAdminAccount { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool HasChangedPassword { get; set; }
}