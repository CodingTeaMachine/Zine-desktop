using System.Data;
using Zine.App.Enums;

namespace Zine.App.FileHelpers;

public static class SymlinkCreator
{

    /// <summary>
    ///     Creates a symlink to the given comic book to the ComicBookLinkDirectory destination
    /// </summary>
    /// <param name="comicBookPath"></param>
    /// <returns></returns>
    /// <exception cref="DataException">Throws when the comic book in the given path doesn't exist</exception>
    public static FileSystemInfo CreateComicBookLink(string comicBookPath)
    {
        if (!File.Exists(comicBookPath))
            throw new DataException("File doesn't exist");

        if (!Directory.Exists(DataPath.ComicBookLinkDirectory))
            Directory.CreateDirectory(DataPath.ComicBookLinkDirectory);

        var filename = Path.GetFileName(comicBookPath);
        var fileExtension = Path.GetExtension(filename);
        var fileCounter = 1;

        while (Path.Exists(Path.Join(DataPath.ComicBookLinkDirectory, filename)))
        {
            filename = Path.GetFileNameWithoutExtension(filename) + $"-{fileCounter}" + fileExtension;
            fileCounter++;
        }

        return Create(Path.Join(DataPath.ComicBookLinkDirectory, filename), comicBookPath);
    }

    private static FileSystemInfo Create(string linkPath, string originalPath)
    {
        return File.CreateSymbolicLink(linkPath, originalPath);
    }
}
