using Zine.App.Repositories;

namespace Zine.App.Services;

public interface IService<T>
{
    protected IRepository<T> Repository { get; }
    public IEnumerable<T> GetAll();
}
