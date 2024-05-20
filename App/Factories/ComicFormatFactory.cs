using Zine.App.Enums;

namespace Zine.App.Factories;

public abstract class ComicFormatFactory
{
    public static readonly List<string> ComicFileExtensions = [".cb7", ".cba", ".cbr" ,".cbt", ".cbz"];
    public static ComicFormat GetFromFileExtension(string? fileExtension)
    {
        return fileExtension?.ToLower() switch
        {
            null => ComicFormat.Unknown,
            ".cb7" => ComicFormat._7Z,
            ".cba" => ComicFormat.Ace,
            ".cbr" => ComicFormat.Rar,
            ".cbt" => ComicFormat.Tar,
            ".cbz" => ComicFormat.Zip,
            _ => ComicFormat.Unknown
        };
    }
}
