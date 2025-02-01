using MudBlazor;
using SharpCompress.Archives;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;
using Zine.App.Helpers;

namespace Zine.App.Domain.ComicBookPageInformation;

public class ComicBookPageInformationService(IComicBookPageInformationRepository repository) : IComicBookPageInformationService
{
	public void CreateMany(string comicBookPathOnDisk, int comicBookId)
	{
		var infoExtractor = new CompressedComicBookInformationExtractor();
		using IArchive comicBookFile = ArchiveFactory.Open(comicBookPathOnDisk);
		List<IArchiveEntry> pages = infoExtractor.GetPages(comicBookFile).ToList();
		var pageNamingFormat = CompressedFileHandler.GetPageNamingFormat(comicBookFile);

		IArchiveEntry coverImage = infoExtractor.GetCoverImage(comicBookFile);
		float coverAspectRatio = Image.GetAspectRatio(coverImage);
		double minAspectRatio = coverAspectRatio * 0.7;

		var pagesToCreate = pages.Select<IArchiveEntry, ComicBookPageInformation>((page, index) =>
			new ComicBookPageInformation()
			{
				ComicBookId = comicBookId,
				PageFileName = page.Key!,
				PageType = GetPageType(page, pageNamingFormat, minAspectRatio),
				PageNumberStart = index + 1
			}
		);

		repository.CreateMany(pagesToCreate);
	}

	private static PageType GetPageType(IArchiveEntry page, ComicBookPageNamingFormatName format, double minAspectRatio)
	{
		if (CompressedFileHandler.IsCoverImage(page, format))
			return PageType.Cover;

		if(CompressedFileHandler.IsRearImage(page, format))
			return PageType.Rear;

		if (Image.GetAspectRatio(page) <= minAspectRatio)
			return PageType.Double;

		return PageType.Single;
	}
}
