namespace Zine.App.Domain.ComicBook.FormatHandler;

public class ComicBookFormatHandlerFactory(string filePath, string coverImageDirectory)
{
	public IComicBookFormatHandler GetFromFormat(ComicBookFormat format)
	{
		return format switch
		{
			ComicBookFormat.Zip => new ZipFormatHandler(filePath, coverImageDirectory),
			ComicBookFormat._7Z => throw new NotImplementedException(),
			ComicBookFormat.Ace => throw new NotImplementedException(),
			ComicBookFormat.Rar => throw new NotImplementedException(),
			ComicBookFormat.Tar => throw new NotImplementedException(),
			ComicBookFormat.Unknown => throw new NotImplementedException(),
			_ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
		};
	}
}
