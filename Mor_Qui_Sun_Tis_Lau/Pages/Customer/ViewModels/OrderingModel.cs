using System.ComponentModel.DataAnnotations;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Customer.ViewModels;

public class OrderingViewModel
{
    [Required(ErrorMessage = "Building is required")]
    public string Building { get; set; } = string.Empty;

    [Required(ErrorMessage = "Room Number is required")]
    public string RoomNumber { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;
    public decimal Tip { get; set; } = 10m;

    public OrderingViewModel() { }

    public OrderingViewModel(CampusLocation campusLocation, decimal tip)
    {
        Building = campusLocation.Building;
        RoomNumber = campusLocation.RoomNumber;
        Notes = campusLocation.Notes;
        Tip = tip;
    }
}