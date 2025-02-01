using Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBookInformation;

public class ComicBookInformationService(IComicBookInformationRepository repository, ILoggerService logger) : IComicBookInformationService
{
	public ComicBookInformation Create(string comicBookPathOnDisk, int comicBookId)
	{

		var pageNamingFormat = CompressionFormatFactory.GetFromFile(comicBookPathOnDisk);
		ComicBookInformationFactory comicBookInformationFactory = new(logger);
		var coverImageName = comicBookInformationFactory.SaveCoverImage(comicBookPathOnDisk, comicBookId.ToString());

		return repository.Create(
			comicBookId,
			coverImageName,
			(int)pageNamingFormat,
			comicBookInformationFactory.GetNumberOfPages(comicBookPathOnDisk)
		);
	}
}
