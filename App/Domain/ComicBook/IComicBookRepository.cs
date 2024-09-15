namespace Zine.App.Domain.ComicBook;

public interface IComicBookRepository
{
	public IEnumerable<ComicBook> GetAllByGroupId(int groupId);
	public ComicBook? GetById(int comicId);
	public ComicBook Create(string name, string fileUri,  ComicBookInformation.ComicBookInformation cbInfo, int groupId);
	public void CreateMany(IEnumerable<ComicBook> comicBooks);
	public bool AddToGroup(int groupId, int comicBookId);
	public void MoveAll(int currentGroupId, int newGroupId);
	public bool Rename(int comicBookId, string newName);
	public void DeleteAllFromGroup(int groupId);
	public bool Delete(int comicId);
}
