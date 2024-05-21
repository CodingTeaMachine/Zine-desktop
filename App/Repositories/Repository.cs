using Microsoft.EntityFrameworkCore;
using Zine.App.Database;

namespace Zine.App.Repositories;

public class Repository(IDbContextFactory<ZineDbContext> contextFactory)
{
    private ZineDbContext? Context { get; set; }

    public ZineDbContext GetDbContext()
    {
        return Context ??= contextFactory.CreateDbContext();
    }

}
