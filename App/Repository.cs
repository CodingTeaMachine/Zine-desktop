using Microsoft.EntityFrameworkCore;
using Zine.App.Database;

namespace Zine.App;

public class Repository(IDbContextFactory<ZineDbContext> contextFactory)
{
    private ZineDbContext? Context { get; set; }

    protected ZineDbContext GetDbContext()
    {
        return Context ??= contextFactory.CreateDbContext();
    }

}
