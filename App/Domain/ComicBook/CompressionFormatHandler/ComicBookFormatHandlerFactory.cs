namespace Zine.App.Domain.ComicBook.CompressionFormatHandler;

public class ComicBookFormatHandlerFactory(string filePath, string coverImageDirectory)
{
	public IComicBookCompressionFormatHandler GetFromFormat(ComicBookCompressionFormat compressionFormat)
	{
		return compressionFormat switch
		{
			ComicBookCompressionFormat.Zip => new ZipCompressionCompressionFormatHandler(filePath, coverImageDirectory),
			ComicBookCompressionFormat._7Z => throw new NotImplementedException(),
			ComicBookCompressionFormat.Ace => throw new NotImplementedException(),
			ComicBookCompressionFormat.Rar => throw new NotImplementedException(),
			ComicBookCompressionFormat.Tar => throw new NotImplementedException(),
			ComicBookCompressionFormat.Unknown => throw new NotImplementedException(),
			_ => throw new ArgumentOutOfRangeException(nameof(compressionFormat), compressionFormat, null)
		};
	}
}
