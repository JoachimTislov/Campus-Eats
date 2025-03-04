using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;

public class Admin : User
{
    /* 

        This class is only here to explicitly create "Admins" in the code, and
        because only admin use the following constructor

    */

    public Admin(string username)  // Used in admin
    {
        UserName = username;

        // Bypass email validation
        var guid = Guid.NewGuid();
        Email = $"{guid}@{guid}.com";
    }
}