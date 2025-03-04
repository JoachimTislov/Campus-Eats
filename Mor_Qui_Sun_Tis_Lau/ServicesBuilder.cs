using MediatR;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Repositories.AdminInvitationR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Repositories.CourierRequestR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.ProductS;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.Session;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.IdentityCustomOptions;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Mor_Qui_Sun_Tis_Lau.Infrastructure;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau;

public class ServicesBuilder()
{
    public static void InitializeServices(IServiceCollection services, string? connectionString)
    {
        GeneralInitialization(services, connectionString);
        AddDependencyInjections(services);
        ConfigureAuthentication(services);
    }

    public static void GeneralInitialization(IServiceCollection services, string? connectionString)
    {
        services.AddMediatR(typeof(Program));
        services.AddSession();
        services.AddHttpContextAccessor();

        // Configure DbContext with SQLite
        services.AddDbContext<ShopContext>(options =>
            options.UseSqlite(connectionString));

        services.AddDefaultIdentity<User>(IdentityOptionsConfiguration.ConfigureIdentityOptions)
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ShopContext>();

        // Add Razor Pages and SignalR
        services.AddRazorPages();
        services.AddSignalR();
    }

    public static void AddDependencyInjections(IServiceCollection services)
    {
        // Dependency injections
        services.AddScoped(typeof(IDbRepository<>), typeof(DbRepository<>));

        services.AddScoped<IFulfillmentRepository, FulfillmentRepository>();
        services.AddScoped<IFulfillmentService, FulfillmentService>();

        services.AddScoped<IInvoicingRepository, InvoicingRepository>();

        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartService, CartService>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ICourierRequestRepository, CourierRequestRepository>();
        services.AddScoped<IAdminInvitationRepository, AdminInvitationRepository>();

        services.AddScoped<IOrderingRepository, OrderingRepository>();
        services.AddScoped<IOrderingService, OrderingService>();

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddSingleton<IEmailService, EmailService>();

        services.AddSingleton<IStripeProductService, StripeProductService>();
        services.AddSingleton<IStripeSessionService, StripeSessionService>();
    }

    public static void ConfigureAuthentication(IServiceCollection services)
    {
        // Configure third-party authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        })
        .AddCookie()
        .AddGoogle(options =>
        {
            options.ClientId = Environment.GetEnvironmentVariable("GoogleClientId")
                ?? throw new InvalidOperationException("Google ClientId is missing in configuration");
            options.ClientSecret = Environment.GetEnvironmentVariable("GoogleClientSecret")
                ?? throw new InvalidOperationException("Google ClientSecret is missing in configuration");
            options.SaveTokens = true;
            options.CallbackPath = "/signin-google";
            options.Events = new OAuthEvents
            {
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/?error=access_denied");
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        })
        .AddFacebook(options =>
        {
            options.AppId = Environment.GetEnvironmentVariable("FacebookAppId")
                ?? throw new InvalidOperationException("Facebook AppId is missing in configuration");
            options.AppSecret = Environment.GetEnvironmentVariable("FacebookAppSecret")
                ?? throw new InvalidOperationException("Facebook AppSecret is missing from configuration");
            options.SaveTokens = true;
            options.Events = new OAuthEvents
            {
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/?error=access_denied");
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        });

        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(5);
        });

        // Configure Cookie settings
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/";
            options.LogoutPath = "/";
            options.AccessDeniedPath = "/";
        });
    }
}


