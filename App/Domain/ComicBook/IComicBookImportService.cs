using Zine.App.Enums;

namespace Zine.App.Domain.ComicBook;

public interface IComicBookImportService
{
	public void ImportFromDisk(ImportType importType, string pathOnDisk, int groupId, bool recursiveImport = false);

}
