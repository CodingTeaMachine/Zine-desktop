using System.Data;
using System.IO.Compression;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Helpers;

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
		var outputPath = Path.Join(CoverImageDirectory, filename);

		try
		{
			var resizedImageData = Image.GetResizedImage(coverImage,  240, 176);
			File.WriteAllBytes(outputPath, resizedImageData);
		}
		catch (Exception e) when(e is DataException or NotSupportedException)
		{
			//Simply save the original image
			coverImage.ExtractToFile(outputPath);
			Console.WriteLine("Error: " + e.Message);
		}



		return filename;
	}
}
