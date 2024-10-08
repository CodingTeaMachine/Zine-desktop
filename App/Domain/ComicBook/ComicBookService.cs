using System.IO.Compression;
using Zine.App.Domain.Group;
using Zine.App.Enums;
using Zine.App.FileHelpers;
using Zine.App.Helpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook;

public class ComicBookService(
    IComicBookRepository comicBookRepository,
    IGroupService groupService,
    ILoggerService logger
    ): IComicBookService
{

    public IEnumerable<ComicBook> GetAllByGroupId(int? groupId = null)
    {
        return comicBookRepository
            .GetAllByGroupId(groupId)
            .Select(cb =>
            {
                if (cb.Information.MovedOrDeleted)
                {
                    logger.Warning($"{cb.FileUri} moved/deleted");
                }

                //Check if the cover image exists, and if not, regenerate it.
                if (!cb.Information.MovedOrDeleted && !File.Exists(Path.Join(DataPath.ComicBookCoverDirectory, cb.Information.CoverImage)))
                {
                    logger.Warning($"Regenerating cover image for: {cb.Name}");
                    new ComicBookInformationFactory().GetCoverImage(cb.FileUri, cb.Information.CompressionFormat);
                }


                return cb;
            });
    }

    public ComicBook? GetById(int comicId)
    {
        return comicBookRepository.GetById(comicId);
    }

    public bool AddToGroup(int? groupId, int comicBookId)
    {
        var comicBook = GetById(comicBookId);
        if (comicBook == null)
        {
            logger.Warning($"Could not find comic with id: {comicBookId}");
            return false;
        }

        if (groupId == null)
        {
            return comicBookRepository.AddToGroup(null, comicBookId);
        }

        var group = groupService.GetById(groupId.Value);

        if (group != null)
            return comicBookRepository.AddToGroup(groupId, comicBookId);

        logger.Warning($"Could not find group with id: {groupId}");
        return false;

    }

    public void MoveAll(int currentGroupId, int? newGroupId)
    {
        comicBookRepository.MoveAll(currentGroupId, newGroupId);
    }

    public bool Rename(int comicBookId, string newName)
    {
        return comicBookRepository.Rename(comicBookId, newName);
    }

    public bool Delete(int comicId)
    {
        var imageName = comicBookRepository.GetById(comicId)!.Information.CoverImage;
        var deleteResult =  comicBookRepository.Delete(comicId);

        //Delete the corresponding cover image for the comic book
        if (deleteResult)
        {
            var pathToDeleteFileFrom = Path.Join(DataPath.ComicBookCoverDirectory, imageName);
            logger.Information($"ComicBookService.Delete: Deleting cover image for: {comicId} at {pathToDeleteFileFrom}");
            File.Delete(pathToDeleteFileFrom);
        }

        return deleteResult;
    }

    public void ExtractImagesForComicBook(int comicBookId)
    {
        IComicBookService.CleanReadingDirectory();

        Console.WriteLine("Extracting images");

        var comicBook = GetById(comicBookId);

        if (comicBook == null)
        {
            throw new ArgumentException("Comic book doesn't exist");
        }

        if (!Directory.Exists(DataPath.ComicBookReadingDirectory))
            Directory.CreateDirectory(DataPath.ComicBookReadingDirectory);

        using ZipArchive comicBookZip = ZipFile.OpenRead(comicBook.FileUri);

        var entriesToSave = comicBookZip
            .Entries
            .Where(entry => Image.Extensions.Contains(entry.Name.Split('.').Last()));

        foreach (var entry in entriesToSave)
        {
            entry.ExtractToFile(Path.Join(DataPath.ComicBookReadingDirectory, entry.Name));
        }
    }
}
