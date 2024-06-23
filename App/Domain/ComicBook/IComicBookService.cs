using Zine.App.Enums;

namespace Zine.App.Domain.ComicBook;

public interface IComicBookService
{
    public IEnumerable<ComicBook> GetAllByGroupId(int? groupId = null);
    public ComicBook? GetById(int comicId);
    public bool ImportFromDisk(ImportType importType, string pathOnDisk);
    public bool AddToGroup(int? groupId, int targetId);
}
