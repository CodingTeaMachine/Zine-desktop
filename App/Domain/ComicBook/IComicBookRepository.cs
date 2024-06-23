namespace Zine.App.Domain.ComicBook;

public interface IComicBookRepository
{
	public IEnumerable<ComicBook> GetAllByGroupId(int? groupId = null);
	public ComicBook? GetById(int comicId);
	public ComicBook Create(string name, string fileName, int? groupId = null);
	public void CreateMany(IEnumerable<ComicBook> comicBooks);
	public bool AddToGroup(int? groupId, int targetId);
}
