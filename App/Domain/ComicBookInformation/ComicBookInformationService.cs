using Zine.App.Database;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBookInformation;

public class ComicBookInformationService(IComicBookInformationRepository repository, ILoggerService logger) : IComicBookInformationService
{
	public ComicBookInformation Create(string comicBookPathOnDisk, int comicBookId, ComicBookPageInformation.ComicBookPageInformation comicBookPageInformation, ZineDbContext? context = null)
	{
		ComicBookInformationFactory comicBookInformationFactory = new(logger);
		var savedCoverImageName = comicBookInformationFactory.SaveThumbnailToDisc(comicBookPathOnDisk, comicBookPageInformation.PageFileName , comicBookId.ToString());

		return repository.Create(comicBookId, savedCoverImageName, context);
	}
}
