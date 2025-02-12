using Zine.App.Database;

namespace Zine.App.Domain.ComicBookPageInformation;

public interface IComicBookPageInformationService
{

	public IEnumerable<ComicBookPageInformation> CreateMany(string comicBookPathOnDisk, int comicBookId, ZineDbContext? context = null);

}
