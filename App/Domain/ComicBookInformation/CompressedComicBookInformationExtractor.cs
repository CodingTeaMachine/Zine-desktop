using System.Data;
using SharpCompress.Archives;
using Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;
using Zine.App.Helpers;

namespace Zine.App.Domain.ComicBookInformation;

public class CompressedComicBookInformationExtractor
{

	public static IArchiveEntry GetCoverImage(string filePath, string coverImageName)
	{
		IArchive comicBookFile = ArchiveFactory.Open(filePath);
		IArchiveEntry coverImage = comicBookFile.Entries.First(cbFile => Path.GetFileName(cbFile.Key!) == coverImageName);
		return coverImage;
	}

	public IArchiveEntry GetCoverImage(IArchive archive)
	{
		if (!archive.Entries.Any())
			throw new DataException("Empty comic book file");

		PageNamingConvention namingConvention = CompressedFileHandler.GetPageNamingFormat(archive);
		IArchiveEntry coverImage =
			archive.Entries.First(cI => CompressedFileHandler.IsCoverImage(cI, namingConvention));

		return coverImage;
	}

	public IEnumerable<IArchiveEntry> GetPages(IArchive archive)
	{
		return archive.Entries
			.Where(entry => !entry.IsDirectory)
			.Where(file => Image.IsSupported(file.Key!));
	}
}
