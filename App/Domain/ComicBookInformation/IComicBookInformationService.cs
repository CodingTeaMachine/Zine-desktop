using Zine.App.Database;

namespace Zine.App.Domain.ComicBookInformation;

public interface IComicBookInformationService
{
	public ComicBookInformation Create(string comicBookPathOnDisk, int comicBookId, ComicBookPageInformation.ComicBookPageInformation coverPageInformation, ZineDbContext? context = null);
}
