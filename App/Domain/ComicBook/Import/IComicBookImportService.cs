using Zine.App.Enums;

namespace Zine.App.Domain.ComicBook.Import;

public interface IComicBookImportService
{
	public void ImportFromDisk(ImportAction action, int groupId);

}
