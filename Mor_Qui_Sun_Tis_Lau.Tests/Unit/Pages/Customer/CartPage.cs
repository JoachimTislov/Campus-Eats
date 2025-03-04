using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Services;
using Mor_Qui_Sun_Tis_Lau.Pages.Customer;
using Microsoft.AspNetCore.Mvc;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Customer;
public class CartPageTests
{
    private readonly Mock<ICartService> _mockCartService = new();
    private readonly CartModel _cartPageModel;

    public CartPageTests()
    {
        _cartPageModel = new(_mockCartService.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        ], "mock"));

        var mockSession = new Mock<ISession>();

        var httpContext = new DefaultHttpContext
        {
            User = user,
            Session = mockSession.Object
        };

        _cartPageModel.PageContext = new PageContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task OnGetAsync_ShouldLoadCart()
    {
        (Guid item1, Guid item2) = (Guid.NewGuid(), Guid.NewGuid());
        var cartId = Guid.NewGuid();
        var cart = new Cart(cartId);
        cart.AddItem(item1, "item1", 5, 2, string.Empty, string.Empty);
        cart.AddItem(item2, "item2", 10, 1, string.Empty, string.Empty);

        _mockCartService
            .Setup(c => c.GetCartIdFromSessionAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<ISession>()))
            .ReturnsAsync(cartId);

        _mockCartService
            .Setup(c => c.GetCartAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cart);

        _mockCartService
            .Setup(c => c.GetCartSubtotal(It.IsAny<Guid>()))
            .ReturnsAsync(cart.GetSubtotal());

        await _cartPageModel.OnGetAsync();

        _mockCartService.Verify(c => c.GetCartIdFromSessionAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<ISession>()), Times.Once);
        _mockCartService.Verify(c => c.GetCartAsync(It.Is<Guid>(id => id == cartId)), Times.Once);
        Assert.Equal(cart.Items.ToList(), _cartPageModel.CartItems);
        Assert.Equal(cart.GetSubtotal(), _cartPageModel.CartSubtotal);
    }

    [Fact]
    public async Task OnPostCheckoutCartAsync_ShouldRedirectToOrdering_WhenCartIsNotEmpty()
    {
        var cartId = Guid.NewGuid();
        var cart = new Cart(cartId);
        cart.AddItem(Guid.NewGuid(), "Item1", 10, 1, "", "");
        _mockCartService.Setup(s => s.GetCartIdFromSessionAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<ISession>())).ReturnsAsync(cartId);
        _mockCartService.Setup(s => s.GetCartAsync(cartId)).ReturnsAsync(cart);
        _mockCartService.Setup(s => s.CheckoutCart(cartId, cart.Items.ToList(), It.IsAny<ClaimsPrincipal>(), It.IsAny<ISession>())).ReturnsAsync(Guid.NewGuid());

        var result = await _cartPageModel.OnPostCheckoutCartAsync();

        var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(UrlProvider.Customer.Ordering, redirectToPageResult.PageName);
    }

    [Fact]
    public async Task OnPostCheckoutCartAsync_ShouldRedirectToCart_WhenCartIsEmpty()
    {
        var cartId = Guid.NewGuid();
        var cart = new Cart(cartId);
        _mockCartService.Setup(s => s.GetCartIdFromSessionAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<ISession>())).ReturnsAsync(cartId);
        _mockCartService.Setup(s => s.GetCartAsync(cartId)).ReturnsAsync(cart);

        var result = await _cartPageModel.OnPostCheckoutCartAsync();

        Assert.IsType<RedirectToPageResult>(result);
    }
}