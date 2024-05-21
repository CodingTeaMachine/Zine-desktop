using Zine.App.Enums;
using Zine.App.Model.DB;

namespace Zine.App.Services;

public interface IComicBookService : IService<ComicBook>
{

    public bool ImportFromDisk(ImportType importType, string pathOnDisk);

    protected bool ImportFileFromDisk(string pathOnDisk);
    protected bool ImportDirectoryFromDisk(string pathOnDisk);
}
