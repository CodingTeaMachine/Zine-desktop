using Zine.App.Enums;
using Zine.App.Model.DB;

namespace Zine.App.Services;

public interface IComicBookService
{
    public IEnumerable<ComicBook> GetAll();
    public bool ImportFromDisk(ImportType importType, string pathOnDisk);
}
