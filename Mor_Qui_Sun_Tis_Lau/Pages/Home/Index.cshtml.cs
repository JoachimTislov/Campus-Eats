using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Pages.Home.ViewModels;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Home;

public class IndexModel(IMediator mediator, IUserService userService, IUserRepository userRepository, ILogger<IndexModel> logger, IAdminService adminService, IProductRepository productRepository) : PageModel
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(userService));
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly ILogger<IndexModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IAdminService _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

    public bool ShowRegister { get; set; }
    public bool UserIsACourier { get; set; }
    public bool UserIsAnAdmin { get; set; }
    public CourierRoleRequest? CourierRoleRequest { get; set; }

    public User? ShopUser { get; set; }
    public List<FoodItem> FoodItems { get; set; } = [];

    public async Task OnGetAsync()
    {
        await LoadIndexData();
    }

    public virtual void ClearModelStateAndValidateViewModel(object ViewModel)
    {
        // Prevent model state issues in view models from affecting each other
        ModelState.Clear(); TryValidateModel(ViewModel, nameof(ViewModel));
    }

    [BindProperty]
    public LoginViewModel LoginViewModel { get; set; } = new();

    public async Task<IActionResult> OnPostLoginAsync()
    {
        ClearModelStateAndValidateViewModel(LoginViewModel);

        if (ModelState.IsValid)
        {
            var user = await _userRepository.GetUserByEmail(LoginViewModel.Email);
            var (succeeded, errorMessage) = await _userService.Login(user, LoginViewModel.Password);

            if (succeeded)
            {
                _logger.LogInformation("{Email} logged in", LoginViewModel.Email);
                return RedirectToPage(UrlProvider.Customer.Canteen);
            }
            else
            {
                LoginViewModel.ErrorMessage = errorMessage;
            }
        }
        return Page();
    }

    [BindProperty]
    public RegisterViewModel RegisterViewModel { get; set; } = new();

    public async Task<IActionResult> OnPostRegisterAsync()
    {
        ClearModelStateAndValidateViewModel(RegisterViewModel);

        ShowRegister = true;

        if (ModelState.IsValid)
        {
            if (RegisterViewModel.Password != RegisterViewModel.Repeat_Password)
            {
                RegisterViewModel.RegisterErrorMessage = "Passwords do not match";
            }
            else
            {
                var (Succeeded, errors) = await _userService.Register(RegisterViewModel.Email, RegisterViewModel.FirstName, RegisterViewModel.LastName, true, RegisterViewModel.Password);
                if (Succeeded)
                {
                    _logger.LogInformation("{Email} registered", RegisterViewModel.Email);
                    return RedirectToPage();
                }
                else
                {
                    RegisterViewModel.Errors = errors;
                }
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnGetExternalLoginCallbackAsync(string? returnUrl = null, string? error = null)
    {
        if (!string.IsNullOrEmpty(error))
        {
            ModelState.AddModelError(string.Empty, "External login failed. Access was denied or an error occurred during login.");
            // Optionally, redirect the user to a specific page or show a message
            return RedirectToPage();
        }
        var result = await _userService.LoginWithThirdParty(HttpContext);

        return result ? LocalRedirect(returnUrl ?? "/Canteen") : Page();
    }

    public IActionResult OnGetExternalLoginAsync(string provider)
    {
        var authenticationScheme = provider switch
        {
            "Facebook" => FacebookDefaults.AuthenticationScheme,
            "Google" => GoogleDefaults.AuthenticationScheme,
            _ => throw new ArgumentException("Invalid provider", nameof(provider))
        };

        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/?handler=ExternalLoginCallback"
        }, authenticationScheme);
    }

    public async Task<IActionResult> OnPostApplyForCourierAsync(string resume)
    {
        await LoadShopUser();

        if (ShopUser != null) await _mediator.Publish(new UserAppliedForCourierRole(ShopUser.Id, resume));

        return RedirectToPage();
    }

    private async Task LoadShopUser()
    {
        ShopUser = await _userRepository.GetUserByHttpContext(HttpContext);
    }

    private async Task LoadIndexData()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            await LoadShopUser();
            if (ShopUser != null)
            {
                CourierRoleRequest = await _adminService.GetCourierRoleRequestByUserId(ShopUser.Id);
            }
            UserIsACourier = await _userService.CheckIfUserIsAssignedRole(User, "Courier");
            UserIsAnAdmin = await _userService.CheckIfUserIsAssignedRole(User, "Admin");

            if (UserIsAnAdmin) FoodItems = _productRepository.GetAllFoodItems();
        }
    }
}