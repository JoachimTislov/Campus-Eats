using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Seeders;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Infrastructure.DataSeeding.Seeders;

public class AdminDataSeederTests
{
    private readonly Mock<IAdminService> _mockAdminService = new();

    private readonly AdminDataSeeder _adminDataSeeder = new();
    private const string AdminName = "Admin";

    [Fact]
    public async Task SeedAdminData_ShouldCreateAdminAccount_WhenAdminIsNull()
    {
        Admin? admin = null;
        _mockAdminService
            .Setup(m => m.GetAdminByName(AdminName))
            .ReturnsAsync(admin);

        await _adminDataSeeder.SeedAdminData(_mockAdminService.Object);

        _mockAdminService.Verify(m => m.GetAdminByName(AdminName), Times.Once);
        _mockAdminService.Verify(m => m.CreateAdminAccount(AdminName), Times.Once);
    }

    [Fact]
    public async Task SeedAdminData_ShouldNotCreateAdminAccount_WhenAdminExist()
    {
        Admin admin = new(AdminName);
        _mockAdminService
            .Setup(m => m.GetAdminByName(AdminName))
            .ReturnsAsync(admin);

        await _adminDataSeeder.SeedAdminData(_mockAdminService.Object);

        _mockAdminService.Verify(m => m.GetAdminByName(AdminName), Times.Once);
        _mockAdminService.Verify(m => m.CreateAdminAccount(AdminName), Times.Never);
    }
}