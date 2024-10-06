namespace Zine.App.Domain.ComicBook.CompressionFormatHandler;

public abstract class ComicBookCompressionFormatFactory
{
    public static readonly List<string> ComicFileExtensions = [".cb7", ".cba", ".cbr" ,".cbt", ".cbz"];
    public static ComicBookCompressionFormat GetFromFileExtension(string? fileExtension)
    {
        return fileExtension?.ToLower() switch
        {
            null => ComicBookCompressionFormat.Unknown,
            ".cb7" => ComicBookCompressionFormat._7Z,
            ".cba" => ComicBookCompressionFormat.Ace,
            ".cbr" => ComicBookCompressionFormat.Rar,
            ".cbt" => ComicBookCompressionFormat.Tar,
            ".cbz" => ComicBookCompressionFormat.Zip,
            _ => ComicBookCompressionFormat.Unknown
        };
    }

    public static ComicBookCompressionFormat GetFromFilePathOrName(string filePathOrName)
    {
        var extenstion = Path.GetExtension(filePathOrName);
        return GetFromFileExtension(extenstion);
    }
}
