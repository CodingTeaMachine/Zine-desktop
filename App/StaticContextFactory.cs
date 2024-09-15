using Microsoft.EntityFrameworkCore;
using Zine.App.Database;

namespace Zine.App;

public static class StaticContextFactory
{
    private static ZineDbContext? Context { get; set; }

    public static ZineDbContext GetDbContext(IDbContextFactory<ZineDbContext> contextFactory)
    {
        return Context ??= contextFactory.CreateDbContext();
    }

    public static void DisposeDbContext()
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss:fff} Disposed DBContext");
        Context?.Dispose();
        Context = null;
    }
}
