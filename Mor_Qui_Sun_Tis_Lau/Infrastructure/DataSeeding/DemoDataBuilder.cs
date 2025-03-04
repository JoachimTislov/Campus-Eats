using MediatR;
using Microsoft.AspNetCore.Identity;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Cleaners;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Seeders;

namespace Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding;

public class DemoDataBuilder
{
    public static async Task BuildDemoData(IServiceProvider provider, bool IsDevelopment)
    {
        // Remove existing carts
        ShopContext context = provider.GetRequiredService<ShopContext>();
        await ClearCartsTable.RemoveCartEntries(context);

        // Remove existing notifications
        await ClearAlertNotificationsTable.RemoveAlertNotificationEntries(context);

        // Creates FoodItems
        IMediator mediator = provider.GetRequiredService<IMediator>();
        await FoodItemsSeeder.SeedData(context, mediator, IsDevelopment);

        IUserService userService = provider.GetRequiredService<IUserService>();
        IUserRepository userRepository = provider.GetRequiredService<IUserRepository>();
        UserSeeder userSeeder = new(userRepository, userService);

        // Creates Roles
        RoleManager<IdentityRole<Guid>> roleManager = provider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        await userSeeder.SeedUserRoles(roleManager);

        // Creates Users
        await userSeeder.SeedUserData(context);

        var orderSeeder = new OrderSeeder(userRepository);
        await orderSeeder.SeedData(context, mediator);

        IOrderingService orderingService = provider.GetRequiredService<IOrderingService>();
        IInvoicingRepository invoicingRepository = provider.GetRequiredService<IInvoicingRepository>();
        var invoiceSeeder = new InvoiceSeeder(invoicingRepository, orderingService, mediator);
        await invoiceSeeder.SeedData();
        await invoiceSeeder.SetInvoicesToPaid();

        // Creates the admin account
        var adminService = provider.GetRequiredService<IAdminService>();
        AdminDataSeeder adminDataSeeder = new();
        await adminDataSeeder.SeedAdminData(adminService);
    }
}