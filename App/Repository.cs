using Microsoft.EntityFrameworkCore;
using Zine.App.Database;

namespace Zine.App;

public class Repository(IDbContextFactory<ZineDbContext> contextFactory)
{

    private int _referenceCounter = 0;

    protected ZineDbContext GetDbContext()
    {
        _referenceCounter++;
	    return StaticContextFactory.GetDbContext(contextFactory);
    }

    protected void DisposeDbContext()
    {
        if (_referenceCounter > 0)
            _referenceCounter--;
        else
            StaticContextFactory.DisposeDbContext();

    }
}
