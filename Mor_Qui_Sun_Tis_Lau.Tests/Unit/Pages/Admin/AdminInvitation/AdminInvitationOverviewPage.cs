using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.AdminInvitation;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Admin.AdminInvitation;

public class AdminInvitationOverviewPageTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();

    private readonly AdminInvitationOverviewModel _adminInvitationOverviewModel;

    public AdminInvitationOverviewPageTests()
    {
        _adminInvitationOverviewModel = new(_mockUserRepository.Object);
    }

    [Fact]
    public async Task OnGetAsync()
    {
        List<User> customers = [new User()];
        _mockUserRepository
            .Setup(m => m.GetCustomers())
            .ReturnsAsync(customers);

        await _adminInvitationOverviewModel.OnGetAsync();

        _mockUserRepository.Verify(m => m.GetCustomers(), Times.Once);

        Assert.NotNull(_adminInvitationOverviewModel.Customers);
        Assert.NotEmpty(_adminInvitationOverviewModel.Customers);
    }
}