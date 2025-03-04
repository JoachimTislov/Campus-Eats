using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.Profile.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Profile;

[Authorize(Roles = "Customer, Courier")]
public class ProfileModel(IUserRepository userRepository) : PageModel
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    public User? ShopUser { get; set; }

    [BindProperty]
    public ProfileViewModel ViewModel { get; set; } = new();

    [BindProperty]
    public AddressViewModel AddressViewModel { get; set; } = new();

    private async Task InitializeProfilePageInformation(bool isUpdating = false)
    {
        ShopUser = await _userRepository.GetUserByHttpContext(HttpContext);
        if (ShopUser != null && !isUpdating)
        {
            ViewModel.FirstName = ShopUser.FirstName;
            ViewModel.LastName = ShopUser.LastName;
            ViewModel.Email = ShopUser.Email!;
            ViewModel.PhoneNumber = ShopUser.PhoneNumber ?? "";

            AddressViewModel.Address = ShopUser.Address.AddressLine;
            AddressViewModel.City = ShopUser.Address.City;
            AddressViewModel.PostalCode = ShopUser.Address.PostalCode;
        }
    }

    public async Task OnGetAsync()
    {
        await InitializeProfilePageInformation();
    }

    public async Task<IActionResult> OnPostProfileAsync()
    {
        await InitializeProfilePageInformation(true);

        if (ModelState.IsValid)
        {
            if (ShopUser == null) return RedirectToPage(UrlProvider.Index);

            var (success, user) = await _userRepository.UpdateUserProfile(ShopUser.Email!, ViewModel.FirstName, ViewModel.LastName, ViewModel.PhoneNumber);
            if (success)
            {
                ShopUser = user;
            }
            else
            {
                return RedirectToPage(UrlProvider.Index);
            }
        }
        return RedirectToPage(UrlProvider.Profile);
    }

    public async Task<IActionResult> OnPostAddressAsync()
    {
        await InitializeProfilePageInformation(true);

        if (ShopUser == null) return RedirectToPage(UrlProvider.Index);

        if (AddressViewModel.Address != null && AddressViewModel.Address != "")
        {
            await _userRepository.UpdateUserAddress(ShopUser.Email!, AddressViewModel.Address, AddressViewModel.City, AddressViewModel.PostalCode);
        }

        return RedirectToPage(UrlProvider.Profile);
    }
}

