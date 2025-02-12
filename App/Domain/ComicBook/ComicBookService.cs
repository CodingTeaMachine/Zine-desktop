using MudBlazor;
using SharpCompress;
using SharpCompress.Archives;
using Zine.App.Domain.ComicBookPageInformation;
using Zine.App.Domain.Group;
using Zine.App.Enums;
using Zine.App.Exceptions;
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

    public IEnumerable<ComicBook> GetAllByGroupId(int groupId)
    {
        return comicBookRepository
            .GetAllByGroupId(groupId)
            .Select(cb =>
            {
                switch (cb.Information.FileMovedOrDeleted)
                {
                    case true:
                        logger.Warning($"{cb.FileUri} moved/deleted");
                        break;
                    //Check if the cover image exists, and if not, regenerate it.
                    case false when !File.Exists(Path.Join(DataPath.ComicBookCoverDirectory, cb.Information.SavedCoverImageFileName)):
                        logger.Warning($"Regenerating cover image for: {cb.Title}");
                        var coverImage = cb.Pages.First(page => page.PageType == PageType.Cover);
                        new ComicBookInformationFactory().SaveThumbnailToDisc(coverImage.PageFileName ,cb.FileUri, cb.Id.ToString());
                        break;
                }


                return cb;
            });
    }

    public ComicBook? GetById(int comicId)
    {
        return comicBookRepository.GetById(comicId);
    }

    public void AddToGroup(int groupId, int comicBookId)
    {
        var comicBook = GetById(comicBookId);
        if (comicBook == null)
        {
            throw new HandledAppException($"Could not find comic with id: {comicBookId}", Severity.Warning);
        }

        groupService.GetById(groupId);

        comicBookRepository.AddToGroup(groupId, comicBookId);
    }

    public void MoveAll(int currentGroupId, int newGroupId)
    {
        comicBookRepository.MoveAll(currentGroupId, newGroupId);
    }

    public void Rename(int comicBookId, string newName)
    {
        comicBookRepository.Rename(comicBookId, newName);
    }

    public bool Delete(int comicId)
    {
        return comicBookRepository.Delete(comicId);
    }

    public void ExtractImagesOfComicBook(int comicBookId)
    {
        CleanReadingDirectory();

        Console.WriteLine("Extracting images");

        var comicBook = GetById(comicBookId);

        if (comicBook == null)
        {
            throw new ArgumentException("Comic book doesn't exist");
        }

        if (!Directory.Exists(DataPath.ComicBookReadingDirectory))
            Directory.CreateDirectory(DataPath.ComicBookReadingDirectory);

        using IArchive comicBookFile = ArchiveFactory.Open(comicBook.FileUri);

        comicBookFile
            .Entries
            .Where(entry => !entry.IsDirectory && Image.IsSupported(entry.Key!))
            .ForEach(entry => entry.WriteToDirectory(DataPath.ComicBookReadingDirectory));
    }

    public static void CleanReadingDirectory()
    {
        if(!Directory.Exists(DataPath.ComicBookReadingDirectory))
            return;

        foreach (var file in Directory.GetFiles(DataPath.ComicBookReadingDirectory))
        {
            File.Delete(file);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public IEnumerable<ComicBook> SearchByTitle(string searchTerm)
    {
        return comicBookRepository.SearchByTitle(searchTerm);
    }
}
