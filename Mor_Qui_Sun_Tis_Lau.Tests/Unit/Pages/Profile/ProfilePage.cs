using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Pages.Profile;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers.Data;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Profile;
public class ProfilePageTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<ICartService> _mockCartService = new();
    private readonly ProfileModel _profilePageModel;

    public ProfilePageTests()
    {
        _profilePageModel = new(_mockUserRepository.Object);
    }

    [Theory]
    [MemberData(nameof(ProfileTestData.TestCases), MemberType = typeof(ProfileTestData))]
    public async Task OnPostProfileAsync_ModelStateTest(string firstName, string lastName, string email, string phoneNumber, bool shouldSucceed, string? expectedError)
    {
        _profilePageModel.ViewModel = new()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber
        };
        var cart = new Cart(Guid.NewGuid());

        _mockCartService
            .Setup(c => c.GetCartAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cart);

        if (expectedError != null)
        {
            _profilePageModel.ModelState.AddModelError(expectedError, expectedError);
        }

        await _profilePageModel.OnPostProfileAsync();

        Assert.Equal(shouldSucceed, _profilePageModel.ModelState.IsValid);
        if (expectedError != null)
        {
            Assert.Equal(expectedError, _profilePageModel.ModelState[expectedError]!.Errors[0].ErrorMessage);
        }
    }
}