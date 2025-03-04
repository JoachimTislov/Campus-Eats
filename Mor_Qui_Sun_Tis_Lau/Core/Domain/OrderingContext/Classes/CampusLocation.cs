using Microsoft.EntityFrameworkCore;
using Mor_Qui_Sun_Tis_Lau.Pages.Customer.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;

[Owned]
public class CampusLocation
{
    public string Building { get; protected set; } = string.Empty;
    public string RoomNumber { get; protected set; } = string.Empty;
    public string Notes { get; protected set; } = string.Empty;

    public CampusLocation() { }

    public CampusLocation(string building, string roomNumber, string notes)
    {
        Building = building;
        RoomNumber = roomNumber;
        Notes = notes;
    }

    public CampusLocation(OrderingViewModel orderingViewModel)
    {
        Building = orderingViewModel.Building;
        RoomNumber = orderingViewModel.RoomNumber;
        Notes = orderingViewModel.Notes;
    }

    public string LocationDisplay()
    {
        return $"{Building} - {RoomNumber}";
    }
}