
namespace Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Cleaners;

public class ClearAlertNotificationsTable
{
    public static async Task RemoveAlertNotificationEntries(ShopContext db)
    {
        if (db.AlertNotifications.Any())
        {
            db.RemoveRange(db.AlertNotifications);
            await db.SaveChangesAsync();
        }
    }
}