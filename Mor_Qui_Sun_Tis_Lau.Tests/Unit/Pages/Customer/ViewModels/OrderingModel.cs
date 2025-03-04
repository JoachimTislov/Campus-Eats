using Mor_Qui_Sun_Tis_Lau.Pages.Customer.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Customer.ViewModels;

public class OrderingViewModelTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var orderingViewModel = new OrderingViewModel();

        Assert.Equal(string.Empty, orderingViewModel.Building);
        Assert.Equal(string.Empty, orderingViewModel.RoomNumber);
        Assert.Equal(string.Empty, orderingViewModel.Notes);
        Assert.Equal(10, orderingViewModel.Tip);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var building = "building";
        var roomNumber = "roomNumber";
        var notes = "note";
        var tip = 10m;

        var orderingViewModel = new OrderingViewModel()
        {
            Building = building,
            RoomNumber = roomNumber,
            Notes = notes,
            Tip = tip
        };

        Assert.Equal(building, orderingViewModel.Building);
        Assert.Equal(roomNumber, orderingViewModel.RoomNumber);
        Assert.Equal(notes, orderingViewModel.Notes);
        Assert.Equal(tip, orderingViewModel.Tip);
    }
}