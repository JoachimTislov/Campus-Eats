using Microsoft.AspNetCore.Identity;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Mor_Qui_Sun_Tis_Lau.Infrastructure;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Seeders;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Infrastructure.DataSeeding.Seeders;

public class UserSeederTests
{
    private readonly Mock<FakeRoleManager> _mockRoleManager = new();
    private readonly Mock<FakeUserManager> _mockUserManager = new();
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly ShopContext _shopContext;
    private readonly UserSeeder _userSeeder;

    public UserSeederTests()
    {
        _userSeeder = new(_mockUserRepository.Object, _mockUserService.Object);
        _shopContext = DbTest.CreateContext();
    }

    [Fact]
    public async Task SeedUsers_ShouldSeedUsers_WhenTheyDoNotExist()
    {
        _shopContext.Users.RemoveRange(_shopContext.Users);
        await _shopContext.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(new List<User>().AsQueryable());
        _mockUserService.Setup(m => m.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<UserLoginInfo>()))
            .ReturnsAsync((true, []));
        _mockUserRepository.Setup(m => m.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync((string email) => new User { Email = email });
        _mockUserRepository.Setup(m => m.UpdateUserProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((string email, string firstName, string lastName, string phoneNumber) => (true, new User { Email = email }));

        _shopContext.Roles.Add(new IdentityRole<Guid> { Name = "Courier" });
        await _shopContext.SaveChangesAsync();

        await _userSeeder.SeedUserData(_shopContext);

        _mockUserService.Verify(m => m.Register("edwinl@testmail.com", "Edwin", "Larson", false, "Test1234*", null), Times.Once);
        _mockUserService.Verify(m => m.Register("johnhenry@emailtest.net", "John", "Henry", false, "Test1234*", null), Times.Once);
        _mockUserService.Verify(m => m.Register("uldman@internationaltest.org", "Karl", "Uldman", false, "Test1234*", null), Times.Once);
        _mockUserService.Verify(m => m.AssignRoleToUserAsync(It.Is<User>(u => u.Email == "johnhenry@emailtest.net"), "Courier"), Times.Once);
    }

    [Fact]
    public async Task SeedUsers_ShouldNotSeedUsers_WhenTheyExist()
    {
        _shopContext.Users.RemoveRange(_shopContext.Users);
        await _shopContext.SaveChangesAsync();

        _shopContext.Users.AddRange(new List<User>
        {
            new() { Email = "edwinl@testmail.com", FirstName = "Edwin", LastName = "Larson" },
            new() { Email = "johnhenry@emailtest.net", FirstName = "John", LastName = "Henry" },
            new() { Email = "uldman@internationaltest.org", FirstName = "Karl", LastName = "Uldman" }
        });
        await _shopContext.SaveChangesAsync();

        await _userSeeder.SeedUserData(_shopContext);

        // Assert
        _mockUserService.Verify(m => m.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<UserLoginInfo>()), Times.Never);
        _mockUserService.Verify(m => m.AssignRoleToUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SeedUsers_ShouldNotAssignRole_WhenRolesAreNotSet()
    {
        _shopContext.Users.RemoveRange(_shopContext.Users);
        await _shopContext.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(new List<User>().AsQueryable());
        _mockUserService.Setup(m => m.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<UserLoginInfo>()))
            .ReturnsAsync((true, []));
        _mockUserRepository.Setup(m => m.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync((string email) => new User { Email = email });

        await _shopContext.SaveChangesAsync();

        await _userSeeder.SeedUserData(_shopContext);

        _mockUserService.Verify(m => m.Register("edwinl@testmail.com", "Edwin", "Larson", false, "Test1234*", null), Times.Once);
        _mockUserService.Verify(m => m.Register("johnhenry@emailtest.net", "John", "Henry", false, "Test1234*", null), Times.Once);
        _mockUserService.Verify(m => m.Register("uldman@internationaltest.org", "Karl", "Uldman", false, "Test1234*", null), Times.Once);
        _mockUserService.Verify(m => m.AssignRoleToUserAsync(It.Is<User>(u => u.Email == "johnhenry@emailtest.net"), "Courier"), Times.Never);
    }

    [Fact]
    public async Task SeedRoles_ShouldSeedRoles_WhenTheyDoNotExist()
    {
        _mockRoleManager.Setup(m => m.Roles).Returns(new List<IdentityRole<Guid>>().AsQueryable());
        _mockRoleManager.Setup(m => m.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        await _userSeeder.SeedUserRoles(_mockRoleManager.Object);

        _mockRoleManager.Verify(m => m.CreateAsync(It.Is<IdentityRole<Guid>>(r => r.Name == "Customer")), Times.Once);
        _mockRoleManager.Verify(m => m.CreateAsync(It.Is<IdentityRole<Guid>>(r => r.Name == "Courier")), Times.Once);
        _mockRoleManager.Verify(m => m.CreateAsync(It.Is<IdentityRole<Guid>>(r => r.Name == "Admin")), Times.Once);
    }

    [Fact]
    public async Task SeedRoles_ShouldNotSeedRoles_WhenTheyExist()
    {
        _mockRoleManager.Setup(m => m.Roles).Returns(new List<IdentityRole<Guid>>().AsQueryable());
        _mockRoleManager.Setup(m => m.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        await _userSeeder.SeedUserRoles(_mockRoleManager.Object);

        _mockRoleManager.Verify(m => m.CreateAsync(It.IsAny<IdentityRole<Guid>>()), Times.Never);
    }
}