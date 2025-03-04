using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Services;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.ProductCRUD;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.ProductCRUD.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Admin.ProductCRUD;

public class CreateProductPageTests
{
    private readonly Mock<IProductService> _mockProductRepository = new();

    private readonly CreateProductModel _createProductModel;

    public CreateProductPageTests()
    {
        _createProductModel = new(_mockProductRepository.Object);
    }

    [Fact]
    public async Task OnPostAsync_ShouldReturnPage_WhenModelStateIsInvalid()
    {
        _createProductModel.ModelState.AddModelError("key", "error");

        var result = await _createProductModel.OnPostAsync();

        Assert.IsType<PageResult>(result);

        _mockProductRepository.Verify(m => m.CreateFoodItemAsync(It.IsAny<FoodItemViewModel>()), Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_ShouldCreateFoodItemAndRedirect_WhenModelStateIsValid()
    {
        var result = await _createProductModel.OnPostAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);

        Assert.Equal(UrlProvider.Index, redirect.PageName);

        _mockProductRepository.Verify(m => m.CreateFoodItemAsync(It.IsAny<FoodItemViewModel>()), Times.Once);
    }
}