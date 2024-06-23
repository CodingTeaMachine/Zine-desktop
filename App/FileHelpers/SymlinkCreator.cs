using System.Data;
using Zine.App.Logger;

namespace Zine.App.FileHelpers;

public static class SymlinkCreator
{
    private const string ComicBookLinkDirectory = "Data/ComicBookLinks";


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

        if (!Directory.Exists(ComicBookLinkDirectory))
            Directory.CreateDirectory(ComicBookLinkDirectory);

        string filename = Path.GetFileName(comicBookPath);
        string fileExtension = Path.GetExtension(filename);
        int fileCounter = 1;

        while (Path.Exists(Path.Join(ComicBookLinkDirectory, filename)))
        {
            filename = Path.GetFileNameWithoutExtension(filename) + $"-{fileCounter}" + fileExtension;
            fileCounter++;
        }

        return Create(Path.Join(ComicBookLinkDirectory, filename), comicBookPath);
    }

    private static FileSystemInfo Create(string linkPath, string originalPath)
    {
        return File.CreateSymbolicLink(linkPath, originalPath);
    }
}
