using System.Data;
using SharpCompress.Archives;
using Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;
using Zine.App.Helpers;

namespace Zine.App.Domain.ComicBookInformation;

public class CompressedComicBookInformationExtractor
{

	public IArchiveEntry GetCoverImage(string filePath)
	{
		using IArchive comicBookFile = ArchiveFactory.Open(filePath);
		return GetCoverImage(comicBookFile);
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
