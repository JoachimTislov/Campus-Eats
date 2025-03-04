
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.ProductCRUD.ViewModels;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Admin.ProductCRUD.ViewModels;

public class FoodItemViewModelTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var foodItemModel = new FoodItemViewModel();

        Assert.Equal(Guid.Empty, foodItemModel.Id);
        Assert.Equal(string.Empty, foodItemModel.Name);
        Assert.Equal(string.Empty, foodItemModel.Description);
        Assert.Equal("filler.jpg", foodItemModel.ImageLink);
        Assert.Equal(0, foodItemModel.Price);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var foodItem = new FoodItem("name", "description", 20m, "imageLink");

        var foodItemModel = new FoodItemViewModel(foodItem);

        Assert.Equal(foodItem.Id, foodItemModel.Id);
        Assert.Equal(foodItem.Name, foodItemModel.Name);
        Assert.Equal(foodItem.Description, foodItemModel.Description);
        Assert.Equal(foodItem.ImageLink, foodItemModel.ImageLink);
        Assert.Equal(foodItem.Price, foodItemModel.Price);
    }

    [Fact]
    public void Properties_AssertSetters()
    {
        var type = typeof(FoodItemViewModel);

        AssertSetter.AssertPublic(type, nameof(FoodItemViewModel.Id));
        AssertSetter.AssertPublic(type, nameof(FoodItemViewModel.Name));
        AssertSetter.AssertPublic(type, nameof(FoodItemViewModel.Description));
        AssertSetter.AssertPublic(type, nameof(FoodItemViewModel.ImageLink));
        AssertSetter.AssertPublic(type, nameof(FoodItemViewModel.Price));
        AssertSetter.AssertPublic(type, nameof(FoodItemViewModel.Price));
        AssertSetter.AssertPublic(type, nameof(FoodItemViewModel.ImageFile));
    }
}