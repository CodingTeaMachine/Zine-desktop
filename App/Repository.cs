using Microsoft.EntityFrameworkCore;
using Zine.App.Database;

namespace Zine.App;

public class Repository(IDbContextFactory<ZineDbContext> contextFactory)
{
    protected ZineDbContext GetDbContext()
    {
	    return StaticContextFactory.GetDbContext(contextFactory);
    }

    protected void DisposeDbContext()
    {
        StaticContextFactory.DisposeDbContext();
    }
}
