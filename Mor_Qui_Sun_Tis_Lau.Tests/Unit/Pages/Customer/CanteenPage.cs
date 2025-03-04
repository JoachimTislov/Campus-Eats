
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Pages.Customer;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Customer;

public class CanteenPageTests
{
    private readonly Mock<ICartService> _mockCartService = new();
    private readonly Mock<IProductRepository> _mockProductRepository = new();

    private readonly CanteenModel _canteenModel;

    public CanteenPageTests()
    {
        _canteenModel = new(_mockCartService.Object, _mockProductRepository.Object);

        var httpContext = new DefaultHttpContext
        {
            Session = new Mock<ISession>().Object
        };

        _canteenModel.PageContext = new()
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public void OnGet()
    {
        var foodItem = new FoodItem("name", "description", 29m, "imageLink");
        List<FoodItem> foodItems = [foodItem];

        _mockProductRepository
            .Setup(m => m.GetAllFoodItems())
            .Returns(foodItems);

        _canteenModel.OnGet();

        Assert.NotEmpty(_canteenModel.FoodItems);
        Assert.Equal(foodItem, _canteenModel.FoodItems[0]);
    }

    [Fact]
    public async Task OnPostAddToCartAsync()
    {
        var itemId = Guid.NewGuid();
        var name = "name";
        var price = 20m;
        var count = 5;
        var imageLink = "";
        var stripe_productId = "stripe_productId";

        var cartId = Guid.NewGuid();
        _mockCartService
            .Setup(m => m.GetCartIdFromSessionAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<ISession>()))
            .ReturnsAsync(cartId);

        var result = await _canteenModel.OnPostAddToCartAsync(itemId, name, price, count, imageLink, stripe_productId);

        var redirect = Assert.IsType<RedirectToPageResult>(result);

        Assert.Null(redirect.PageName); // Redirect to the same page. Runs OnGet 

        _mockCartService.Verify(m => m.GetCartIdFromSessionAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<ISession>()), Times.Once);
        _mockCartService.Verify(m => m.AddItemToCartAsync(itemId, name, price, count, cartId, imageLink, stripe_productId), Times.Once);
    }
}