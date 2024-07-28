using Microsoft.Data.Sqlite;
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
	public IEnumerable<Group> GetAllByParentId(int? parentId = null)
	{
		// Return only the first 4 comic books from the group
		return groupRepository.GetAllByParentId(parentId)
			.Select(g =>
			{
				g.ComicBooks = g.ComicBooks.Take(4).ToList();

				//Check if the cover image exists, and if not, regenerate it.
				foreach (var cb in g.ComicBooks)
				{
					if (!File.Exists(Path.Join(DataPath.ComicBookCoverDirectory, cb.Information.CoverImage)))
					{
						logger.Warning($"Regenerating cover image for: {cb.Name}");
						new ComicBookInformationFactory().GetCoverImage(cb.FileUri, cb.Information.CompressionFormat);
					}
				}

				return g;
			})
			.ToList();
	}

	public Group? GetById(int groupId)
	{
		return groupRepository.GetById(groupId);
	}


	public Group Create(string newGroupName, int? parentId = null)
	{
		return groupRepository.Create(newGroupName, parentId);
	}

	public bool Rename(int groupId, string newName)
	{
		return groupRepository.Rename(groupId, newName);
	}

	public bool AddToGroup(int? newParentGroupId, int groupId)
	{
		return groupRepository.AddToGroup(newParentGroupId, groupId);
	}

	public void MoveAll(int? currentParentGroupId, int? newParentGroupId)
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
			comicBookRepository.MoveAll(groupId, currentGroup!.ParentGroupId);
			MoveAll(groupId, currentGroup!.ParentGroupId);
		}

		return groupRepository.Delete(groupId);
	}

	public string GetName(int groupId)
	{
		return groupRepository.GetById(groupId)?.Name ?? "";
	}
}
