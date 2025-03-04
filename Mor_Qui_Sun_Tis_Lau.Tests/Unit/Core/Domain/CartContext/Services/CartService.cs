using MediatR;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.CartContext.Services;

public class CartServiceTests
{
    private readonly Mock<ICartRepository> _mockCartRepository = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IMediator> _mockMediator = new();
    private readonly CartService _cartService;

    public CartServiceTests()
    {
        _cartService = new(_mockCartRepository.Object, _mockUserRepository.Object, _mockMediator.Object);
    }

    [Fact]
    public async Task GetCartSubtotal_ShouldReturnCartSubtotal()
    {
        (Guid item1, Guid item2) = (Guid.NewGuid(), Guid.NewGuid());
        var cartId = Guid.NewGuid();
        var cart = new Cart(cartId);
        cart.AddItem(item1, "Test1", 5, 2, string.Empty, string.Empty);
        cart.AddItem(item2, "Test2", 10, 1, string.Empty, string.Empty);

        _mockCartRepository
            .Setup(c => c.GetCartAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cart);

        var subTotal = await _cartService.GetCartSubtotal(cart.Id);

        Assert.Equal(20m, subTotal);
    }

}