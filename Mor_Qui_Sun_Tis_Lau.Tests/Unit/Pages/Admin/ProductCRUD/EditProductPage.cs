using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Services;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.ProductCRUD;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.ProductCRUD.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Admin.ProductCRUD;

public class EditProductPageTests
{
    private readonly Mock<IProductService> _mockProductRepository = new();

    private readonly EditProductModel _editProductModel;

    public EditProductPageTests()
    {
        _editProductModel = new(_mockProductRepository.Object);
    }

    [Fact]
    public async Task OnGetAsync_ShouldReturnNotFound_WhenFoodItemIsNull()
    {
        FoodItem? foodItem = null;
        _mockProductRepository
            .Setup(m => m.GetFoodItemByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(foodItem);

        var result = await _editProductModel.OnGetAsync(It.IsAny<Guid>());

        Assert.IsType<RedirectToPageResult>(result);

        Assert.NotNull(_editProductModel.ViewModel);
    }

    [Fact]
    public async Task OnGetAsync_ShouldReturnPageAndCreateFoodItemViewModel_WhenFoodItemIsNotNull()
    {
        var foodItem = new FoodItem("name", "description", 20m, "imageLink");
        _mockProductRepository
            .Setup(m => m.GetFoodItemByIdAsync(foodItem.Id))
            .ReturnsAsync(foodItem);

        var result = await _editProductModel.OnGetAsync(foodItem.Id);

        Assert.IsType<PageResult>(result);

        Assert.Equal(foodItem.Name, _editProductModel.ViewModel.Name);
        Assert.Equal(foodItem.Description, _editProductModel.ViewModel.Description);
        Assert.Equal(foodItem.Price, _editProductModel.ViewModel.Price);
        Assert.Equal(foodItem.ImageLink, _editProductModel.ViewModel.ImageLink);
    }

    [Fact]
    public async Task OnPostEditAsync_ShouldReturnPage_WhenModelStateIsInvalid()
    {
        _editProductModel.ModelState.AddModelError("key", "error");

        var result = await _editProductModel.OnPostEditAsync();

        Assert.IsType<PageResult>(result);

        _mockProductRepository.Verify(m => m.EditFoodItemAsync(It.IsAny<FoodItemViewModel>()), Times.Never);
    }

    [Fact]
    public async Task OnPostEditAsync_ShouldEditFoodItemAndRedirect_WhenModelStateIsValid()
    {
        var result = await _editProductModel.OnPostEditAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);

        Assert.Equal(UrlProvider.Index, redirect.PageName);

        _mockProductRepository.Verify(m => m.EditFoodItemAsync(It.IsAny<FoodItemViewModel>()), Times.Once);
    }

    [Fact]
    public async Task OnPostDeleteAsync_ShouldRedirectToIndex_WhenEditFoodItemEditsAFoodItemSuccessfully()
    {
        var result = await _editProductModel.OnPostDeleteAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);

        Assert.Equal(UrlProvider.Index, redirect.PageName);

        _mockProductRepository.Verify(m => m.DeleteFoodItemAsync(It.IsAny<Guid>()), Times.Once);
    }
}