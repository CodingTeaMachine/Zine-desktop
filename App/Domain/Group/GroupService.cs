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
	public Group? LoadForLibraryPage(int parentId)
	{
		var loadedGroup = groupRepository.LoadForLibraryPage(parentId);

		if (loadedGroup == null)
			return null;

		// Return only the first 4 comic books from the group
		loadedGroup.ChildGroups = loadedGroup.ChildGroups
			.Select(g =>
			{
				g.ComicBooks = g.ComicBooks.Take(4).ToList();

				//Check if the cover image exists, and if not, regenerate it.
				foreach (var cb in g.ComicBooks)
				{
					var comicBookCoverImagePath = Path.Join(DataPath.ComicBookCoverDirectory, cb.Information.CoverImage);
					if (File.Exists(comicBookCoverImagePath))
						continue;

					logger.Warning($"Regenerating cover image for: {cb.Name}");
					new ComicBookInformationFactory().GetCoverImage(cb.FileUri, cb.Information.CompressionCompressionFormat);
				}

				return g;
			})
			.ToList();

		return loadedGroup;
	}

	public Group? GetById(int groupId)
	{
		return groupRepository.GetById(groupId);
	}


	public Group Create(string newGroupName, int parentId)
	{
		return groupRepository.Create(newGroupName, parentId);
	}

	public bool Rename(int groupId, string newName)
	{
		return groupRepository.Rename(groupId, newName);
	}

	public bool AddToGroup(int newParentGroupId, int groupId)
	{
		return groupRepository.AddToGroup(newParentGroupId, groupId);
	}

	public void MoveAll(int currentParentGroupId, int newParentGroupId)
	{
		groupRepository.MoveAll(currentParentGroupId, newParentGroupId);
	}

	public bool Delete(int groupId, bool deleteAllContent)
	{
		var currentGroup = groupRepository.GetByIdWithChildGroups(groupId);

		if (deleteAllContent)
		{

			//Delete the corresponding cover images for the comic books
			comicBookRepository
				.GetAllByGroupId(groupId)
				.ToList()
				.ForEach(comicBook =>
			{
				var pathToDeleteFileFrom =
					Path.Join(DataPath.ComicBookCoverDirectory, comicBook.Information.CoverImage);
				logger.Information($"ComicBookService.DeleteAllFromGroup: Deleting cover image for: {comicBook.Id} at {pathToDeleteFileFrom}");
				File.Delete(pathToDeleteFileFrom);
			});

			comicBookRepository.DeleteAllFromGroup(groupId);
			currentGroup!
				.ChildGroups
				.ToList()
				.ForEach(g => Delete(g.Id, true));
		}
		else
		{
			comicBookRepository.MoveAll(groupId, currentGroup!.ParentGroupId!.Value);
			MoveAll(groupId, currentGroup.ParentGroupId.Value);
		}

		return groupRepository.Delete(groupId);
	}

	public string GetName(int groupId)
	{
		return groupRepository.GetById(groupId)?.Name ?? "";
	}
}
