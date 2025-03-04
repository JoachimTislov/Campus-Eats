using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;

namespace Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Seeders;

public class AdminDataSeeder
{
    public async Task SeedAdminData(IAdminService adminService)
    {
        string adminName = "Admin";
        if (await adminService.GetAdminByName(adminName) == null)
        {
            await adminService.CreateAdminAccount(adminName);
        }
    }
}