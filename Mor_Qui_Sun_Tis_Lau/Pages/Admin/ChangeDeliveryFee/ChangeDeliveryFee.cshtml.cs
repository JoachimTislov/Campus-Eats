using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Admin.ChangeDeliveryFee;

[Authorize(Roles = "Admin")]
public class ChangeDeliveryFeeModel(IAdminService adminService) : PageModel
{
    private readonly IAdminService _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));

    [BindProperty]
    [Required(ErrorMessage = "Delivery Fee is required")]
    [Range(0, 500, ErrorMessage = "Must be in the range 0-500")]
    public decimal DeliveryFee { get; set; }

    public IActionResult OnGet()
    {
        LoadDeliveryFee();
        return Page();
    }

    private void LoadDeliveryFee()
    {
        DeliveryFee = _adminService.GetDeliveryFee();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _adminService.SetDeliveryFee(DeliveryFee);

        return RedirectToPage(UrlProvider.Admin.ChangeDeliveryFee);
    }
}