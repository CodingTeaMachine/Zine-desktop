using Microsoft.EntityFrameworkCore;
using Zine.App.Database;

namespace Zine.App.Repositories;

public class Repository(IDbContextFactory<ZineDbContext> contextFactory)
{
    private ZineDbContext? _context { get; set; }

    public ZineDbContext GetDbContext()
    {
        return _context ??= contextFactory.CreateDbContext();
    }

}
