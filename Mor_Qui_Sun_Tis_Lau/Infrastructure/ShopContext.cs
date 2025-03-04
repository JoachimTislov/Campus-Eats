using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

namespace Mor_Qui_Sun_Tis_Lau.Infrastructure;

public class ShopContext(DbContextOptions<ShopContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<FoodItem> FoodItems { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;

    public DbSet<CourierRoleRequest> CourierRoleRequests { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderLine> OrderLines { get; set; } = null!;
    public DbSet<AdminInvitation> AdminInvitations { get; set; } = null!;

    public DbSet<Offer> Offers { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;

    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<AlertNotification> AlertNotifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Cart>()
            .HasMany(cart => cart.Items)
            .WithOne()
            .HasForeignKey(cartItem => cartItem.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(builder);
    }
}