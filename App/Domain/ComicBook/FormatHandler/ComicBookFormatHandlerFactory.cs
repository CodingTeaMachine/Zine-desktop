using System.Drawing.Printing;

namespace Zine.App.Domain.ComicBook.FormatHandler;

public class ComicBookFormatHandlerFactory(string filePath, string coverImageDirectory)
{
	private string FilePath { get; set; } = filePath;
	private string CoverImageDirectory { get; set; } = coverImageDirectory;

	public IComicBookFormatHandler GetFromFormat(ComicBookFormat format)
	{
		return format switch
		{
			ComicBookFormat.Zip => new ZipFormatHandler(FilePath, CoverImageDirectory),
			ComicBookFormat._7Z => throw new NotImplementedException(),
			ComicBookFormat.Ace => throw new NotImplementedException(),
			ComicBookFormat.Rar => throw new NotImplementedException(),
			ComicBookFormat.Tar => throw new NotImplementedException(),
			ComicBookFormat.Unknown => throw new NotImplementedException(),
			_ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
		};
	}
}
