using SharpCompress.Archives;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;
using Zine.App.Enums;
using Zine.App.Helpers;
using Zine.App.Logger;

namespace Zine.App.FileHelpers;

public class ComicBookInformationFactory(ILoggerService? logger = null)
{
	public string SaveCoverImage(string pathOnDisk, string outputFileName)
	{
		if (!Directory.Exists(DataPath.ComicBookCoverDirectory))
			Directory.CreateDirectory(DataPath.ComicBookCoverDirectory);


		var formatHandler = new CompressedFileHandler(pathOnDisk, DataPath.ComicBookCoverDirectory);
		var coverImageName  = formatHandler.ExtractCoverImage(outputFileName);

		logger?.Information($"Importing cover image for: {Path.GetFileNameWithoutExtension(pathOnDisk)}. Image name: {coverImageName}");

		return coverImageName;
	}

	/// <summary>
	/// Returns the number of pages in a comic book
	/// 1. Check the cover image aspect ratio
	/// 2. Calculate the max diff in aspect ratios
	/// 3. The minimum number of pages is the amount of files
	/// 4. If a pages aspect ratio is bigger then the max diff in aspect ratios add 1 more to the total number of pages
	/// </summary>
	/// <param name="pathOnDisk"></param>
	/// <returns></returns>
	public int GetNumberOfPages(string pathOnDisk)
	{
		using IArchive comicBookFile = ArchiveFactory.Open(pathOnDisk);

		var infoExtractor = new CompressedComicBookInformationExtractor();

		IArchiveEntry coverImage = infoExtractor.GetCoverImage(comicBookFile);
		float coverAspectRatio = Image.GetAspectRatio(coverImage);
		double minAspectRatio = coverAspectRatio * 0.7;

		List<IArchiveEntry> pages = comicBookFile.Entries.Where(file => Image.Extensions.Contains(file.Key?.Split('.').Last())).ToList();

		return
			pages.Count +
			pages.Count(page =>
				Image.GetAspectRatio(page) <= minAspectRatio
			);
	}

}
