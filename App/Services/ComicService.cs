using Zine.App.Model;
using Zine.App.Repositories;

namespace Zine.App.Services;

public class ComicService(IComicRepository repository): IComicService
{
    public IRepository<Comic> Repository { get; } = repository;

    public IEnumerable<Comic> GetAll()
    {
        var result = Repository.GetAll();
        return result;
    }
}
