using Zine.App.Enums;

namespace Zine.App.Domain.ComicBook;

public interface IComicBookImportService
{
	public List<string>? ImportFromDisk(ImportType importType, string pathOnDisk, int groupId, bool recursiveImport = false);

}
