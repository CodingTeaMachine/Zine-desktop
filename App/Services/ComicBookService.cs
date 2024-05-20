using System.Data;
using Zine.App.Enums;
using Zine.App.Factories;
using Zine.App.FileHelpers;
using Zine.App.Logger;
using Zine.App.Model;
using Zine.App.Repositories;

namespace Zine.App.Services;

public class ComicBookService(IComicBookRepository comicBookRepository, ILoggerService logger): IComicBookService
{

    public IEnumerable<Comic> GetAll()
    {
        var result = comicBookRepository.GetAll();
        return result;
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
            return true;
        }
        catch (DataException)
        {
            return false;
        }
    }

    public bool ImportDirectoryFromDisk(string pathOnDisk)
    {
        List<string> comicBookFiles = Directory.EnumerateFiles(pathOnDisk, "*.cb?", SearchOption.AllDirectories)
            .Where(filePath => ComicFormatFactory.ComicFileExtensions.Contains(Path.GetExtension(filePath)))
            .ToList();

        foreach (var file in comicBookFiles)
        {
            try
            {
                SymlinkCreator.CreateComicBookLink(file);
            }
            catch (DataException)
            {
                //TODO: Display errors to user
                logger.Warning($"Could not import file: {Path.GetFileName(file)}");
            }
        }

        return true;
    }
}
