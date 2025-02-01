using System.Data;
using SharpCompress.Archives;
using Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;

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

		ComicBookPageNamingFormatName namingFormatName = CompressedFileHandler.GetPageNamingFormat(archive);
		IArchiveEntry coverImage =
			archive.Entries.First(cI => CompressedFileHandler.IsCoverImage(cI, namingFormatName));

		return coverImage;

	}
}
