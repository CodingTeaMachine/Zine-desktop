namespace Zine.App.Domain.ComicBook;

public interface IComicBookService
{
    public IEnumerable<ComicBook> GetAllByGroupId(int? groupId = null);
    public ComicBook? GetById(int comicId);
    public bool AddToGroup(int? groupId, int targetId);
}
