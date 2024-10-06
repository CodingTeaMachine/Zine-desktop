using System.Data;
using System.IO.Compression;
using Zine.App.Helpers;

namespace Zine.App.Domain.ComicBookInformation.CompressionFormatHandler.Handlers;

public class ZipCompressionCompressionFormatHandler(string filePath, string coverImageDirectory): GeneralCompressionFormatHandler(coverImageDirectory), IComicBookCompressionFormatHandler
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
			var resizedImageData = Image.GetResizedImage(coverImage, 369, 240);
			File.WriteAllBytes(outputPath, resizedImageData);
		}
		catch (NotSupportedException)
		{
			//Simply save the original image
			coverImage.ExtractToFile(outputPath);
		}
		catch (DataException)
		{
			//Simply save the original image
			coverImage.ExtractToFile(outputPath);
		}

		return filename;
	}
}
