using Zine.App.Enums;
using Zine.App.Model;

namespace Zine.App.Services;

public interface IComicBookService : IService<Comic>
{

    public bool ImportFromDisk(ImportType importType, string pathOnDisk);

    protected bool ImportFileFromDisk(string pathOnDisk);
    protected bool ImportDirectoryFromDisk(string pathOnDisk);
}
