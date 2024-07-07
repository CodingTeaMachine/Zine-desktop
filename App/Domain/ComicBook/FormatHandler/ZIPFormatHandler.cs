using System.Data;
using System.IO.Compression;
using Zine.App.Domain.ComicBookInformation;

namespace Zine.App.Domain.ComicBook.FormatHandler;

public class ZipFormatHandler(string filePath, string coverImageDirectory): GeneralFormatHandler(coverImageDirectory), IComicBookFormatHandler
{

	public string ExtractCoverImage()
	{
		using ZipArchive comicBookZip = ZipFile.OpenRead(filePath);
		if (comicBookZip.Entries.Count <= 0) throw new DataException("Empty comic book file");


		ComicBookPageNamingFormatName namingFormatName = GetPageNamingFormat(comicBookZip);
		ZipArchiveEntry coverImage = comicBookZip.Entries.First(cI => IsCoverImage(cI, namingFormatName));
		var filename = GetFilename(coverImage);

		coverImage.ExtractToFile(Path.Join(CoverImageDirectory, filename));
		return filename;
	}
}
