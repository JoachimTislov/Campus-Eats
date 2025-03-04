namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.IdentityCustomOptions;

public class PasswordRequirements
{
    public readonly int MinLength = 8;
    public readonly int UniqueChars = 1;
    public readonly bool RequireDigit = true;
    public readonly bool RequireLowercase = true;
    public readonly bool RequireNonAlphanumeric = true;
    public readonly bool RequireUppercase = true;
}