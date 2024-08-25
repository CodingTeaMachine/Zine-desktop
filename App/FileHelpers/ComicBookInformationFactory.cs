using System.IO.Compression;
using Zine.App.Domain.ComicBook.FormatHandler;
using Zine.App.Enums;
using Zine.App.Helpers;
using Zine.App.Logger;

namespace Zine.App.FileHelpers;

public class ComicBookInformationFactory(ILoggerService? logger = null)
{

	public string GetCoverImage(string pathOnDisk, ComicBookFormat format)
	{
		if (!Directory.Exists(DataPath.ComicBookCoverDirectory))
			Directory.CreateDirectory(DataPath.ComicBookCoverDirectory);


		var formatHandler = new ComicBookFormatHandlerFactory(pathOnDisk, DataPath.ComicBookCoverDirectory).GetFromFormat(format);
		var coverImageName  = formatHandler.ExtractCoverImage();

		logger?.Information($"Importing cover image for: {Path.GetFileNameWithoutExtension(pathOnDisk)}. Image name: {coverImageName}");

		return coverImageName;
	}

	public int GetNumberOfPages(string pathOnDisk)
	{
		using ZipArchive comicBookZip = ZipFile.OpenRead(pathOnDisk);

		return comicBookZip.Entries.Where(file => Image.Extensions.Contains(file.Name.Split('.').Last())).ToList().Count;
	}
}
