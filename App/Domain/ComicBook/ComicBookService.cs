using Zine.App.Domain.Group;
using Zine.App.Enums;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook;

public class ComicBookService(IComicBookRepository comicBookRepository, IGroupService groupService, ILoggerService logger): IComicBookService
{

    public IEnumerable<ComicBook> GetAllByGroupId(int? groupId = null)
    {
        return comicBookRepository
            .GetAllByGroupId(groupId)
            .Select(cb =>
            {

                //Check if the cover image exists, and if not, regenerate it.
                if (!File.Exists(Path.Join(DataPath.ComicBookCoverDirectory, cb.Information.CoverImage)))
                {
                    logger.Warning($"Regenerating cover image for: {cb.Name}");
                    new ComicBookInformationFactory().GetCoverImage(Path.Join(DataPath.ComicBookLinkDirectory, cb.FileName),
                        cb.Information.CompressionFormat);
                }

                return cb;
            });
    }

    public ComicBook? GetById(int comicId)
    {
        return comicBookRepository.GetById(comicId);
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

        if (group != null)
            return comicBookRepository.AddToGroup(groupId, targetId);

        logger.Warning($"Could not find group with id: {groupId}");
        return false;

    }
}
