using Zine.App.Enums;
using Zine.App.Model.DB;

namespace Zine.App.Services;

public interface IComicBookService
{
    public IEnumerable<ComicBook> GetAllByGroupId(int? groupId = null);
    public ComicBook? GetById(int comicId);
    public bool ImportFromDisk(ImportType importType, string pathOnDisk);
    public bool AddToGroup(int groupId, int targetId);
}
