using Microsoft.AspNetCore.Identity;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.IdentityCustomOptions;

public static class IdentityOptionsConfiguration
{
    public static void ConfigureIdentityOptions(IdentityOptions options)
    {
        var PasswordRequirements = new PasswordRequirements();
        // Password settings.
        options.Password.RequireDigit = PasswordRequirements.RequireDigit;
        options.Password.RequireLowercase = PasswordRequirements.RequireLowercase;
        options.Password.RequireNonAlphanumeric = PasswordRequirements.RequireNonAlphanumeric;
        options.Password.RequireUppercase = PasswordRequirements.RequireUppercase;
        options.Password.RequiredLength = PasswordRequirements.MinLength;
        options.Password.RequiredUniqueChars = PasswordRequirements.MinLength;

        // SignIn settings
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;

        // Lockout settings.
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.AllowedForNewUsers = true;

        // User settings.
        options.User.RequireUniqueEmail = true;
    }
}