namespace Zine.App.Services;

public interface IService<T>
{
    public IEnumerable<T> GetAll();
}
