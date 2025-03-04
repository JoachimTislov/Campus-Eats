using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.OrderingContext.Classes;

public class CampusLocationTest
{
    [Fact]
    public void ConstructorWithValues_ShouldAssignValues()
    {
        // Arrange
        var building = "KE";
        var roomNumber = "A-251";
        var notes = "no notes";

        // Act
        var location = new CampusLocation(building, roomNumber, notes);

        // Assert
        Assert.Equal(building, location.Building);
        Assert.Equal(roomNumber, location.RoomNumber);
        Assert.Equal(notes, location.Notes);
    }

    [Fact]
    public void LocationDisplay_ShouldReturnCorrectString()
    {
        CampusLocation location = new("KE", "A-302", "This way");

        string correctReturnString = $"{location.Building} - {location.RoomNumber}";

        Assert.Equal(correctReturnString, location.LocationDisplay());
    }
    
    [Fact]
    public void Properties_ShouldNotAllowUpdates()
    {
        var type = typeof(CampusLocation);

        AssertSetter.AssertProtected(type, nameof(CampusLocation.Building));
        AssertSetter.AssertProtected(type, nameof(CampusLocation.RoomNumber));
        AssertSetter.AssertProtected(type, nameof(CampusLocation.Notes));
    }
}