using SharpCompress.Archives;
using Zine.App.Helpers;

namespace Zine.App.Domain.ComicBookInformation;

public class CompressedComicBookPageInformationExtractor
{

	public static IArchiveEntry GetCoverImage(string filePath, string coverImageName)
	{
		IArchive comicBookFile = ArchiveFactory.Open(filePath);
		IArchiveEntry coverImage = comicBookFile.Entries.First(cbFile => cbFile.Key! == coverImageName);
		return coverImage;
	}

	public IEnumerable<IArchiveEntry> GetPages(IArchive archive)
	{
		return archive.Entries
			.Where(entry => !entry.IsDirectory)
			.Where(file => Image.IsSupported(file.Key!));
	}
}
