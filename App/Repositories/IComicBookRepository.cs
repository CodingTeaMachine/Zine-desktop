using Zine.App.Model;
using Zine.App.Model.DB;

namespace Zine.App.Repositories;

public interface IComicBookRepository
{
	public IEnumerable<ComicBook> GetAll();
	public ComicBook Create(string name, string fileName, int? groupId = null);
	public void CreateMany(IEnumerable<ComicBook> comicBooks);
}
