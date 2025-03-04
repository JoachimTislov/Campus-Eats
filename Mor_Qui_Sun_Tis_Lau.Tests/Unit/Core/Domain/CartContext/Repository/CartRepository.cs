using System.Linq.Expressions;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.CartContext.Repository;

public class CartRepositoryTests
{
    private readonly CartRepository _cartRepository;
    private readonly Mock<IDbRepository<Cart>> _mockDbRepository = new();

    private static Cart ReturnNewCart(Guid cartId, int amountOfCartItemsAdded = 0, int amountOfCartItemsInFirst = 1)
    {
        Cart cart = new(cartId);
        if (amountOfCartItemsAdded == 0) return cart;
        for (int i = 0; i < amountOfCartItemsAdded; i++)
        {
            var cartItemId = Guid.NewGuid();
            var cartItem = new CartItem(cartItemId, "Test " + i, 10, 1, string.Empty, string.Empty);
            if (i == 0)
            {
                for (int j = 0; j < amountOfCartItemsInFirst; j++)
                {
                    cart.AddItem(cartItem.Id, cartItem.Name, cartItem.Price, cartItem.Count, cartItem.ImageLink, cartItem.Stripe_productId);
                }
            }
            else
            {
                cart.AddItem(Guid.NewGuid(), cartItem.Name, cartItem.Price, cartItem.Count, cartItem.ImageLink, cartItem.Stripe_productId);
            }
        }
        return cart;
    }

    public CartRepositoryTests()
    {
        _cartRepository = new CartRepository(_mockDbRepository.Object);
    }

    [Fact]
    public async Task GetCartAsync_ShouldReturnCart_Created()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1);

        _mockDbRepository
            .Setup(d => d.AddAsync(It.IsAny<Cart>()))
            .Returns(Task.CompletedTask);

        _mockDbRepository
        .Setup(d => d.IncludeWhereFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Cart, IEnumerable<CartItem>>>>(),
            It.IsAny<Expression<Func<Cart, bool>>>()))
        .ReturnsAsync((Cart?)null);

        var result = await _cartRepository.GetCartAsync(cartId);

        Assert.NotEqual(cart, result);
        _mockDbRepository.Verify(d => d.IncludeWhereFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Cart, IEnumerable<CartItem>>>>(),
            It.IsAny<Expression<Func<Cart, bool>>>()), Times.Once);
        _mockDbRepository
            .Verify(d => d.AddAsync(It.IsAny<Cart>()), Times.Once);
    }

    [Fact]
    public async Task GetCartAsync_ShouldReturnCart_Found()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1);

        _mockDbRepository
        .Setup(d => d.IncludeWhereFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Cart, IEnumerable<CartItem>>>>(),
            It.IsAny<Expression<Func<Cart, bool>>>()))
        .ReturnsAsync(cart);

        var result = await _cartRepository.GetCartAsync(cartId);

        Assert.Equal(cart, result);
        _mockDbRepository.Verify(d => d.IncludeWhereFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Cart, IEnumerable<CartItem>>>>(),
            It.IsAny<Expression<Func<Cart, bool>>>()), Times.Once);
        _mockDbRepository
            .Verify(d => d.AddAsync(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task GetCartsWithSpecificItemAsync_ShouldReturnCartsWithItem()
    {
        var cartId = Guid.NewGuid();
        var cartId2 = Guid.NewGuid();
        var carts = new List<Cart>()
        {
            ReturnNewCart(cartId, 1),
            ReturnNewCart(cartId2, 1)
        };

        _mockDbRepository
        .Setup(d => d.IncludeWhereToListAsync(
            It.IsAny<Expression<Func<Cart, IEnumerable<CartItem>>>>(),
            It.IsAny<Expression<Func<Cart, bool>>>()))
        .ReturnsAsync(carts);

        var result = await _cartRepository.GetCartsWithSpecificItemAsync(cartId);

        Assert.NotEmpty(result);
        Assert.Equal(carts, result);
        _mockDbRepository.Verify(d => d.IncludeWhereToListAsync(
            It.IsAny<Expression<Func<Cart, IEnumerable<CartItem>>>>(),
            It.IsAny<Expression<Func<Cart, bool>>>()), Times.Once);
        _mockDbRepository
            .Verify(d => d.AddAsync(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCartAsync_ShouldUpdateCart()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1);

        _mockDbRepository
            .Setup(d => d.Update(It.IsAny<Cart>()))
            .Returns(Task.CompletedTask);

        await _cartRepository.UpdateCartAsync(cart);

        _mockDbRepository.Verify(d => d.Update(It.IsAny<Cart>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCartsAsync_ShouldUpdateCart()
    {
        var carts = new List<Cart>()
        {
            ReturnNewCart(Guid.NewGuid(), 1),
            ReturnNewCart(Guid.NewGuid(), 1)
        };

        _mockDbRepository
            .Setup(d => d.UpdateRange(It.IsAny<List<Cart>>()))
            .Returns(Task.CompletedTask);

        await _cartRepository.UpdateCartsAsync(carts);

        _mockDbRepository.Verify(d => d.UpdateRange(It.IsAny<List<Cart>>()), Times.Once());
    }

    [Fact]
    public async Task IncrementCartItemCountAsync_DecrementCartItemCountOrDeleteIfZeroAsync_DeleteCartItemFromCartAsync()
    {
        var cartId = Guid.NewGuid();
        var cartItemId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1);
        cart.AddItem(cartItemId, "Test", 10, 1, string.Empty, string.Empty);

        _mockDbRepository
        .Setup(d => d.IncludeWhereFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Cart, IEnumerable<CartItem>>>>(),
            It.IsAny<Expression<Func<Cart, bool>>>()))
        .ReturnsAsync(cart);

        _mockDbRepository
            .Setup(d => d.Update(It.IsAny<Cart>()))
            .Returns(Task.CompletedTask);

        await _cartRepository.IncrementCartItemCountAsync(cartItemId, cartId);
        await _cartRepository.DecrementCartItemCountOrDeleteIfZeroAsync(cartItemId, cartId);
        await _cartRepository.DeleteCartItemFromCartAsync(cartItemId, cartId);

        _mockDbRepository.Verify(d => d.IncludeWhereFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Cart, IEnumerable<CartItem>>>>(),
            It.IsAny<Expression<Func<Cart, bool>>>()), Times.Exactly(3));
        _mockDbRepository
            .Verify(d => d.AddAsync(It.IsAny<Cart>()), Times.Never);
        _mockDbRepository
            .Verify(d => d.Update(It.IsAny<Cart>()), Times.Exactly(3));
    }

    [Fact]
    public async Task DeleteCartAsync_ShouldDeleteCart()
    {
        var cartId = Guid.NewGuid();
        var cartItemId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1);
        cart.AddItem(cartItemId, "Test", 10, 1, string.Empty, string.Empty);

        await _cartRepository.DeleteCartAsync(cartId);

        _mockDbRepository
            .Verify(d => d.Remove(It.IsAny<Guid>()), Times.Once);
    }

}