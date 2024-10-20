using System.IO.Compression;
using SharpCompress.Archives;
using Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;
using Zine.App.Enums;
using Zine.App.Helpers;
using Zine.App.Logger;

namespace Zine.App.FileHelpers;

public class ComicBookInformationFactory(ILoggerService? logger = null)
{

	//TODO: Remove the format parameter
	public string GetCoverImage(string pathOnDisk, CompressionFormat compressionFormat)
	{
		if (!Directory.Exists(DataPath.ComicBookCoverDirectory))
			Directory.CreateDirectory(DataPath.ComicBookCoverDirectory);


		var formatHandler = new CompressedFileHandler(pathOnDisk, DataPath.ComicBookCoverDirectory);
		var coverImageName  = formatHandler.ExtractCoverImage();

		logger?.Information($"Importing cover image for: {Path.GetFileNameWithoutExtension(pathOnDisk)}. Image name: {coverImageName}");

		return coverImageName;
	}

	public int GetNumberOfPages(string pathOnDisk)
	{
		using IArchive comicBookZip = ArchiveFactory.Open(pathOnDisk);

		return comicBookZip.Entries.Where(file => Image.Extensions.Contains(file.Key?.Split('.').Last())).ToList().Count;
	}
}
