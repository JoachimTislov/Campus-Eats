using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.CartContext.Handlers;

public class FoodPriceNameChangedHandlerTests
{
    private readonly Mock<ICartService> _mockCartService = new();
    private readonly FoodItemNameChangedHandler _handler;

    public FoodPriceNameChangedHandlerTests()
    {
        _handler = new(_mockCartService.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdatePrice()
    {
        var carts = new List<Cart>()
        {
            new(Guid.NewGuid()),
            new(Guid.NewGuid()),
            new(Guid.NewGuid())
        };

        _mockCartService
            .Setup(c => c.GetCartsWithSpecificItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(carts);

        _mockCartService
           .Setup(c => c.UpdateCartsAsync(It.IsAny<List<Cart>>()));

        await _handler.Handle(new FoodItemNameChanged(Guid.NewGuid(), "oldName", "newName"), It.IsAny<CancellationToken>());

        _mockCartService.Verify(c => c.GetCartsWithSpecificItemAsync(It.IsAny<Guid>()), Times.Once());
        _mockCartService.Verify(c => c.UpdateCartsAsync(It.IsAny<List<Cart>>()), Times.Once());
    }
}