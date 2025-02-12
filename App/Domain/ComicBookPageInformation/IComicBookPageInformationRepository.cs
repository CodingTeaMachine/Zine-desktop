using Zine.App.Database;

namespace Zine.App.Domain.ComicBookPageInformation;

public interface IComicBookPageInformationRepository
{
	public IEnumerable<ComicBookPageInformation> CreateMany(IEnumerable<ComicBookPageInformation> comicBookPageInformationList, ZineDbContext? context = null);
}
