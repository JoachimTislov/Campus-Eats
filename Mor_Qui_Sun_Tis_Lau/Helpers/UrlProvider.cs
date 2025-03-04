
namespace Mor_Qui_Sun_Tis_Lau.Helpers;

/*
    Urls used to remove hard corded url strings in redirect to page methods and asp-page tags
*/
public class UrlProvider
{
    public string BaseUrl(string scheme, HostString host) => $"{scheme}://{host}";

    public static class Admin
    {
        public static string InvitationForm => "/Admin/AdminInvitation/AdminInvitationForm";
        public static string InvitationOverview => "/Admin/AdminInvitation/AdminInvitationOverview";
        public static string Login => "/Admin/Auth/AdminLogin";
        public static string CreateProduct => "/Admin/ProductCRUD/CreateProduct";
        public static string EditProduct => "/Admin/ProductCRUD/EditProduct";
        public static string AdminOverview => "/Admin/AdminOverview/AdminOverview";
        public static string CourierRequests => "/Admin/CourierRequests";
        public static string Benefits => "/Admin/Benefits/Benefits";
        public static string ChangeDeliveryFee => "/Admin/ChangeDeliveryFee/ChangeDeliveryFee";

    }

    public static class Courier
    {
        public static string CourierDashboard => "/Courier/CourierDashboard";
        public static string CourierOrderOverview => "/Courier/CourierOrderOverview";
    }

    public static class Customer
    {
        public static string Canteen => "/Customer/Canteen";
        public static string Cart => "/Customer/Cart";
        public static string Ordering => "/Customer/Ordering";
        public static string Orders => "/Customer/Orders";
        public static string OrderDetails => "/Customer/OrderDetails";
        public static string TipCourier => "/Customer/TipCourier";
    }

    public static string ForgotPassword => "/ForgotPassword/ForgotPassword";
    public static string Index => "/Home/Index";
    public static string Notifications => "/Notifications/Notifications";
    public static string Profile => "/Profile/Profile";
    public static string RespondToAdminInvitation => "/RespondToAdminInvitation";
}