using Zine.App.Domain.Group;
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
