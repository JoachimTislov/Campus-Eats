using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.CartContext.Classes;

public class CartTests
{
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
    [Fact]
    public void ConstructorWithValues_ShouldAssignValues()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId);

        // Assert
        Assert.Equal(cartId, cart.Id);
    }

    [Fact]
    public void AddItem_ShouldAddItemToCart()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1);

        Assert.NotNull(cart.Items.First());
        Assert.Equal(1, cart.Items.First().Count);
    }

    [Fact]
    public void AddItem_ShouldAddASecondAndThirdItemToCart()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 3);

        Assert.NotNull(cart.Items.First());
        Assert.Equal(3, cart.Items.Count());
    }

    [Fact]
    public void AddItem_ShouldIncreaseItemCount()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1, 2);

        Assert.NotNull(cart.Items.First());
        Assert.Equal(2, cart.Items.First().Count);
    }

    [Fact]
    public void IncrementCountOfItem_ShouldIncrement()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1);

        Assert.NotNull(cart.Items.First());

        cart.IncrementCountOfItem(cart.Items.First().Id);

        Assert.Equal(2, cart.Items.First().Count);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveOneItemCountFromCart()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1, 2);

        Assert.NotNull(cart.Items.First());

        cart.RemoveItem(cart.Items.First().Id);

        Assert.Equal(1, cart.Items.First().Count);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItemFromCart()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1);

        Assert.NotNull(cart.Items.First());

        cart.RemoveItem(cart.Items.First().Id);

        Assert.Empty(cart.Items.ToList());
    }

    [Fact]
    public void DeleteItemFromCart_ShouldDeleteItemFromCart()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1);

        Assert.NotNull(cart.Items.First());

        cart.RemoveItem(cart.Items.First().Id);

        Assert.Empty(cart.Items.ToList());
    }

    [Fact]
    public void DeleteItemFromCart_ShouldNotDeleteItemFromCart_ItemIsNull()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1);

        Assert.NotNull(cart.Items.First());

        cart.DeleteItemFromCart(Guid.NewGuid());

        Assert.Equal(1, cart.Items.First().Count);
    }

    [Fact]
    public void GetSubtotal_Test()
    {
        var cartId = Guid.NewGuid();
        var cart = ReturnNewCart(cartId, 1, 2);

        var expectedSubTotal = 0m;
        foreach (var item in cart.Items)
        {
            expectedSubTotal += item.Sum;
        }

        Assert.NotNull(cart.Items.First());
        Assert.Equal(expectedSubTotal, cart.GetSubtotal());
    }

    [Fact]
    public void AssertCartSetter()
    {
        var type = typeof(Cart);

        AssertSetter.AssertPrivate(type, nameof(Cart.Id));
    }
}