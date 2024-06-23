using Zine.App.Enums;

namespace Zine.App.Domain.ComicBook;

public interface IComicBookImportService
{
	public bool ImportFromDisk(ImportType importType, string pathOnDisk);

}
