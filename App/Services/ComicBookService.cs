using System.Data;
using Zine.App.Enums;
using Zine.App.Factories;
using Zine.App.FileHelpers;
using Zine.App.Logger;
using Zine.App.Model.DB;
using Zine.App.Repositories;

namespace Zine.App.Services;

public class ComicBookService(IComicBookRepository comicBookRepository, ILoggerService logger): IComicBookService
{

    public IEnumerable<ComicBook> GetAll()
    {
        return comicBookRepository.GetAll();
    }

    public bool ImportFromDisk(ImportType importType ,string pathOnDisk)
    {
        logger.Information($"Importing {(importType == ImportType.Directory ? "file" : "directory")} from: {pathOnDisk}");

        return importType switch
        {
            ImportType.File => ImportFileFromDisk(pathOnDisk),
            ImportType.Directory => ImportDirectoryFromDisk(pathOnDisk),
            _ => throw new ArgumentOutOfRangeException(nameof(importType), importType, null)
        };
    }

    public bool ImportFileFromDisk(string pathOnDisk)
    {
        try
        {
            SymlinkCreator.CreateComicBookLink(pathOnDisk);

            comicBookRepository.Create(
                Path.GetFileNameWithoutExtension(pathOnDisk),
                Path.GetFileName(pathOnDisk)
            );

            return true;
        }
        catch (DataException)
        {
            return false;
        }
    }

    public bool ImportDirectoryFromDisk(string pathOnDisk)
    {
        List<ComicBook> comicBookFiles = Directory.EnumerateFiles(pathOnDisk, "*.cb?", SearchOption.AllDirectories)
            .Where(filePath => ComicFormatFactory.ComicFileExtensions.Contains(Path.GetExtension(filePath)))
            .Select(filePath => new ComicBook {Name = Path.GetFileNameWithoutExtension(filePath), FileName = Path.GetFileName(filePath)})
            .ToList();

        comicBookRepository.CreateMany(comicBookFiles);

        return true;
    }
}
