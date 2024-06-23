using System.Data;
using Zine.App.Domain.Group;
using Zine.App.Enums;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook;

public class ComicBookService(IComicBookRepository comicBookRepository, IGroupService groupService, ILoggerService logger): IComicBookService
{

    public IEnumerable<ComicBook> GetAllByGroupId(int? groupId = null)
    {
        return comicBookRepository.GetAllByGroupId(groupId);
    }

    public ComicBook? GetById(int comicId)
    {
        return comicBookRepository.GetById(comicId);
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

    private bool ImportFileFromDisk(string pathOnDisk)
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

    private bool ImportDirectoryFromDisk(string pathOnDisk)
    {
        List<ComicBook> comicBookFiles = Directory.EnumerateFiles(pathOnDisk, "*.cb?", SearchOption.AllDirectories)
            .Where(filePath => ComicBookFormatFactory.ComicFileExtensions.Contains(Path.GetExtension(filePath)))
            .Select(filePath => new ComicBook {Name = Path.GetFileNameWithoutExtension(filePath), FileName = Path.GetFileName(filePath)})
            .ToList();

        comicBookRepository.CreateMany(comicBookFiles);

        return true;
    }

    public bool AddToGroup(int? groupId, int targetId)
    {
        var comicBook = GetById(targetId);
        if (comicBook == null)
        {
            logger.Warning($"Could not find comic with id: {targetId}");
            return false;
        }

        if (groupId == null)
        {
            return comicBookRepository.AddToGroup(null, targetId);
        }

        var group = groupService.GetById(groupId.Value);
        if (group == null)
        {
            logger.Warning($"Could not find group with id: {groupId}");
            return false;
        }

        return comicBookRepository.AddToGroup(groupId, targetId);
    }
}
