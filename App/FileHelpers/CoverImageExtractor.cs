using Zine.App.Domain.ComicBook.FormatHandler;
using Zine.App.Enums;
using Zine.App.Logger;

namespace Zine.App.FileHelpers;

public class CoverImageExtractor
{

	private ILoggerService? _logger;

	public string ExtractCoverImage(string pathOnDisk, ComicBookFormat format)
	{
		if (!Directory.Exists(DataPath.ComicBookCoverDirectory))
			Directory.CreateDirectory(DataPath.ComicBookCoverDirectory);


		var formatHandler = new ComicBookFormatHandlerFactory(pathOnDisk, DataPath.ComicBookCoverDirectory).GetFromFormat(format);
		var coverImageName  = formatHandler.ExtractCoverImage();

		_logger?.Information($"Importing cover image for: {Path.GetFileNameWithoutExtension(pathOnDisk)}. Image name: {coverImageName}");

		return coverImageName;
	}

	public CoverImageExtractor SetLogger(ILoggerService loggerService)
	{
		_logger = loggerService;
		return this;
	}
}
