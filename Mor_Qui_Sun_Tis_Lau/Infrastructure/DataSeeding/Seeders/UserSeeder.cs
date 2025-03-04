using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Microsoft.AspNetCore.Identity;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;

namespace Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Seeders;

public class UserSeeder(IUserRepository userRepository, IUserService userService)
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    public async Task SeedUserData(ShopContext db)
    {
        if (!db.Users.Any())
        {
            var user_data = new[]
            {
                ("Edwin", "Larson", "edwinl@testmail.com", "98473212"),
                ("John", "Henry", "johnhenry@emailtest.net", "73832937"),
                ("Karl", "Uldman", "uldman@internationaltest.org", "93746592"),
                ("Martin", "Lauritsen", "mslrobo02@gmail.com", "93283782")
            };

            foreach (var (firstName, lastName, email, phoneNumber) in user_data)
            {
                var (success, _) = await _userService.Register(email, firstName, lastName, false, "Test1234*");
                var (updateSuccess, user) = await _userRepository.UpdateUserProfile(email, firstName, lastName, phoneNumber);

                if (success && user != null && firstName == "Edwin" && !db.CourierRoleRequests.Any())
                {
                    var resume = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc bibendum faucibus sem non consequat. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Maecenas maximus euismod.";
                    var roleRequest = new CourierRoleRequest(user.Id, resume);
                    db.CourierRoleRequests.Add(roleRequest);
                    await db.SaveChangesAsync();
                }

                // lastName == "John" to only assign roles to the second user as all users are granted the role of "Customer" on registration
                if (success && user != null && (firstName == "John" || firstName == "Martin"))
                {
                    if (db.Roles.Any()) // This check should not be needed, but doesn't hurt to have it here
                    {
                        await _userService.AssignRoleToUserAsync(user, "Courier");
                    }
                }
            };
        }
    }

    public async Task SeedUserRoles(RoleManager<IdentityRole<Guid>> roleManager)
    {
        if (!roleManager.Roles.Any())
        {
            string[] _roles = ["Customer", "Courier", "Admin"];
            foreach (var role in _roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }
    }
}