
namespace Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Cleaners;

public class ClearCartsTable
{
    public static async Task RemoveCartEntries(ShopContext db)
    {
        if (db.Carts.Any())
        {
            db.RemoveRange(db.Carts);
            await db.SaveChangesAsync();
        }
    }
}