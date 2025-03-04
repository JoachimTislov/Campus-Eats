using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.AdminContext;

public class EventsTests
{
    [Fact]
    public void AdminInvitationEvent()
    {
        var userId = Guid.NewGuid();
        var adminInvitationId = Guid.NewGuid();

        var adminInvitationEvent = new AdminInvitationEvent(userId, adminInvitationId);

        Assert.Equal(userId, adminInvitationEvent.UserId);
        Assert.Equal(adminInvitationId, adminInvitationEvent.AdminInvitationId);
    }

    [Fact]
    public void CourierRoleRequestEvaluated()
    {
        var email = "test@userEmail.com";
        var approved = false;
        var userId = Guid.NewGuid();

        var courierRoleRequestEvaluated = new CourierRoleRequestEvaluated(userId, approved, email);

        Assert.Equal(userId, courierRoleRequestEvaluated.UserId);
        Assert.False(courierRoleRequestEvaluated.Approved);
        Assert.Equal(email, courierRoleRequestEvaluated.UsersEmail);
    }

    [Fact]
    public void ProductCreatedEvent()
    {
        var productId = Guid.NewGuid();
        var name = "name";
        var description = "description";
        var price = 40m;

        var productCreated = new ProductCreated(productId, name, description, price);

        Assert.Equal(productId, productCreated.ProductId);
        Assert.Equal(name, productCreated.Name);
        Assert.Equal(description, productCreated.Description);
        Assert.Equal(price, productCreated.Price);
    }

    [Fact]
    public void ProductEditedEvent()
    {
        var stripe_productId = "stripe_productId";
        var name = "name";
        var description = "description";
        var price = 40m;

        var productCreated = new ProductEdited(stripe_productId, name, description, price);

        Assert.Equal(stripe_productId, productCreated.Stripe_productId);
        Assert.Equal(name, productCreated.Name);
        Assert.Equal(description, productCreated.Description);
        Assert.Equal(price, productCreated.Price);
    }
}