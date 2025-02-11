using Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBookInformation;

public class ComicBookInformationService(IComicBookInformationRepository repository, ILoggerService logger) : IComicBookInformationService
{
	public ComicBookInformation Create(string comicBookPathOnDisk, int comicBookId)
	{
		ComicBookInformationFactory comicBookInformationFactory = new(logger);
		var savedCoverImageName = comicBookInformationFactory.SaveCoverImageToDisc(comicBookPathOnDisk, comicBookId.ToString());

		return repository.Create(comicBookId, savedCoverImageName);
	}
}
