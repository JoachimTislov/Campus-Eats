
namespace Mor_Qui_Sun_Tis_Lau.Tests.Helpers.Data;

public static class UrlTestsData
{
    public static TheoryData<string> UnAuthorizedUrls =>
    [
        "/",
        "/Admin-Login",
        "/Forgot-password",
        "/Error"
    ];

    public static TheoryData<string> AuthorizedUrls =>
    [
        // Courier
        "/Dashboard",
        "/Overview/{validGuid}",

        // Customer
        "/Canteen",
        "/Cart",
        "/Ordering/{validGuid}",
        "/OrderDetails/{validGuid}",
        "/TipCourier/{validGuid}",
        "/Orders",
        "/Notifications",

        // Admin
        "/Admin-invitation-form",
        "/Admin-invitation-overview",
        "/Admin-overview",
        "/Benefits",
        "/Change-deliveryFee",
        "/Create-product",
        "/Edit-product/{validGuid}",
        "/Courier-requests",

        // Both
        "/Profile",
        "/Respond-to-admin-invitation/{validGuid}"
    ];
}