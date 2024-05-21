using Zine.App.Model;
using Zine.App.Model.DB;

namespace Zine.App.Repositories;

public interface IComicBookRepository : IRepository<ComicBook>
{
	public ComicBook Create(string name, string fileName, int? groupId = null);
	public void CreateMany(IEnumerable<ComicBook> comicBooks);
}
