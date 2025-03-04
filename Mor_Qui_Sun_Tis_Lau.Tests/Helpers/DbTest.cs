using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Mor_Qui_Sun_Tis_Lau.Infrastructure;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Helpers;
public static class DbTest
{
    public static ShopContext CreateContext()
    {
        // SQLite in-memory database connection
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ShopContext>()
            .UseSqlite(connection) // in-memory db
            .Options;

        var context = new ShopContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}
