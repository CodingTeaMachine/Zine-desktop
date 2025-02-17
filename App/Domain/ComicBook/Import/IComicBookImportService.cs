using Zine.App.Enums;

namespace Zine.App.Domain.ComicBook.Import;

public interface IComicBookImportService
{
	public List<string>? ImportFromDisk(ImportAction action, int groupId);

}
