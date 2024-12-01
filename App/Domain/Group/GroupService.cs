using Zine.App.Domain.ComicBook;
using Zine.App.Enums;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.Group;

public class GroupService(
	IGroupRepository groupRepository,
	IComicBookRepository comicBookRepository, //Would be nice to use the service, but that would result in circular dependency
	ILoggerService logger) : IGroupService
{

	//Read

	public Group? LoadForLibraryPage(int parentId)
	{
		var loadedGroup = groupRepository.LoadForLibraryPage(parentId);

		if (loadedGroup == null)
			return null;

		// Return only the first 4 comic books from the group
		loadedGroup.ChildGroups = loadedGroup.ChildGroups
			.Select(LoadCoverImagesForComicBooksInGroupCover)
			.ToList();

		return loadedGroup;
	}

	public IEnumerable<Group> List()
	{
		return groupRepository.List();
	}

	public Group GetById(int groupId)
	{
		return groupRepository.GetById(groupId);
	}

	public IEnumerable<Group> SearchByName(string searchTerm)
	{
		var loadedGroups = groupRepository.SearchByName(searchTerm);

		return loadedGroups.Select(LoadCoverImagesForComicBooksInGroupCover);
	}

	public string GetName(int groupId)
	{
		return groupRepository.GetById(groupId)?.Name ?? "";
	}

	private Group LoadCoverImagesForComicBooksInGroupCover(Group g)
	{
		g.ComicBooks = g.ComicBooks.Take(4).ToList();

		//Check if the cover image exists, and if not, regenerate it.
		foreach (var cb in g.ComicBooks)
		{
			var comicBookCoverImagePath = Path.Join(DataPath.ComicBookCoverDirectory, cb.Information.CoverImage);
			if (File.Exists(comicBookCoverImagePath))
				continue;

			logger.Information($"GroupService.LoadCoverImagesForComicBooksInGroupCover: Regenerating cover image for: \"{cb.Title}\"");
			new ComicBookInformationFactory().GetCoverImage(cb.FileUri, cb.Id.ToString());
		}

		return g;
	}


	//Create

	public Group Create(string newGroupName, int parentId)
	{
		return groupRepository.Create(newGroupName, parentId);
	}

	//Update

	public bool Rename(int groupId, string newName)
	{
		return groupRepository.Rename(groupId, newName);
	}

	public bool AddToGroup(int newParentGroupId, int groupId)
	{
		return groupRepository.AddToGroup(newParentGroupId, groupId);
	}

	//Delete

	public bool Delete(int groupId, bool deleteAllContent)
	{
		logger.Information($"GroupService.Delete: GroupId: {groupId}, DeleteAllContent: {deleteAllContent}");
		var currentGroup = groupRepository.GetByIdWithChildGroups(groupId);

		if (!deleteAllContent)
		{
			comicBookRepository.MoveAll(groupId, currentGroup!.ParentGroupId!.Value);
			groupRepository.MoveAll(groupId, currentGroup.ParentGroupId.Value);
			return groupRepository.Delete(groupId);
		}

		currentGroup!
			.ChildGroups
			.ToList()
			.ForEach(cg => Delete(cg.Id, true));

		comicBookRepository.DeleteAllFromGroup(groupId);

		return groupRepository.Delete(groupId);
	}
}
