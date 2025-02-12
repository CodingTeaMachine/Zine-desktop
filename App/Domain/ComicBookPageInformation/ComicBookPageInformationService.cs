using Zine.App.Database;

namespace Zine.App.Domain.ComicBookPageInformation;

public class ComicBookPageInformationService(IComicBookPageInformationRepository repository) : IComicBookPageInformationService
{
	public IEnumerable<ComicBookPageInformation> CreateMany(string comicBookPathOnDisk, int comicBookId, ZineDbContext? context = null)
	{
		var pagesToCreate = PageInfoListFactory.GetPageInfoList(comicBookPathOnDisk, comicBookId);
		return repository.CreateMany(pagesToCreate, context);
	}

}
