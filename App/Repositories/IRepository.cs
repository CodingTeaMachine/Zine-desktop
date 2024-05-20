using Zine.App.Database;

namespace Zine.App.Repositories;

public interface IRepository<T>
{
     // protected ZineDbContext Context { get; }
     public IEnumerable<T> GetAll();

     public ZineDbContext GetDbContext();
}
