using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.CartContext.Classes;

public class CartItemTest
{
    private static CartItem ReturnCartItem(Guid cartItemSku) => new(cartItemSku, "Test", 1m, 5, string.Empty, string.Empty);

    [Fact]
    public void ConstructorWithValues_ShouldAssignValues()
    {
        var cartItemSku = Guid.NewGuid();
        var cartItem = ReturnCartItem(cartItemSku);

        // Assert
        Assert.Equal(cartItemSku, cartItem.Sku);
        Assert.Equal("Test", cartItem.Name);
        Assert.Equal(1m, cartItem.Price);
        Assert.Equal(5, cartItem.Count);
        Assert.Equal(string.Empty, cartItem.Stripe_productId);
    }

    [Fact]
    public void AddOne_Test()
    {
        var cartItemSku = Guid.NewGuid();
        var cartItem = ReturnCartItem(cartItemSku);
        var expectedItemCount = cartItem.Count + 1;

        cartItem.AddOne();

        Assert.Equal(expectedItemCount, cartItem.Count);
    }

    [Fact]
    public void RemoveOne_Test()
    {
        var cartItemSku = Guid.NewGuid();
        var cartItem = ReturnCartItem(cartItemSku);
        var expectedItemCount = cartItem.Count - 1;

        cartItem.RemoveOne();

        Assert.Equal(expectedItemCount, cartItem.Count);
    }

    [Fact]
    public void IsCountOne_ShouldReturnFalse()
    {
        var cartItemSku = Guid.NewGuid();
        var cartItem = ReturnCartItem(cartItemSku);

        Assert.False(cartItem.IsCountOne());
    }

    [Fact]
    public void IsCountOne_ShouldReturnTrue()
    {
        var cartItemSku = Guid.NewGuid();
        var cartItem = ReturnCartItem(cartItemSku);

        while (cartItem.Count > 1)
        {
            cartItem.RemoveOne();
        }

        Assert.True(cartItem.IsCountOne());
    }

    [Fact]
    public void AssertCartSetter()
    {
        var type = typeof(CartItem);

        AssertSetter.AssertPrivate(type, nameof(CartItem.Id));
        AssertSetter.AssertPrivate(type, nameof(CartItem.Sku));
        AssertSetter.AssertPublic(type, nameof(CartItem.Name));
        AssertSetter.AssertPublic(type, nameof(CartItem.Price));
        AssertSetter.AssertPublic(type, nameof(CartItem.Count));
    }
}