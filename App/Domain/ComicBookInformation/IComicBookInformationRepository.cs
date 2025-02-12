using Zine.App.Database;

namespace Zine.App.Domain.ComicBookInformation;

public interface IComicBookInformationRepository
{
	public ComicBookInformation Create(int comicBookId, string savedComicBookFileName, ZineDbContext? context = null);
}
