namespace Zine.App.Domain.ComicBook.FormatHandler;

public abstract class ComicBookFormatFactory
{
    public static readonly List<string> ComicFileExtensions = [".cb7", ".cba", ".cbr" ,".cbt", ".cbz"];
    public static ComicBookFormat GetFromFileExtension(string? fileExtension)
    {
        return fileExtension?.ToLower() switch
        {
            null => ComicBookFormat.Unknown,
            ".cb7" => ComicBookFormat._7Z,
            ".cba" => ComicBookFormat.Ace,
            ".cbr" => ComicBookFormat.Rar,
            ".cbt" => ComicBookFormat.Tar,
            ".cbz" => ComicBookFormat.Zip,
            _ => ComicBookFormat.Unknown
        };
    }

    public static ComicBookFormat GetFromFilePathOrName(string filePathOrName)
    {
        var extenstion = Path.GetExtension(filePathOrName);
        return GetFromFileExtension(extenstion);
    }
}
