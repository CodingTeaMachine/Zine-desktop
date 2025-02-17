using System.Data;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using SharpCompress;
using Zine.App.Database;
using Zine.App.Domain.ComicBook;
using Zine.App.Domain.ComicBookPageInformation;
using Zine.App.Exceptions;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.Group;

public class GroupService(
	ZineDbContext dbContext,
	GenericRepository<Group> repository,
	IComicBookService comicBookService,
	ILoggerService logger) : IGroupService
{

	//Read

	/// <summary>
	///
	/// </summary>
	/// <param name="parentId"></param>
	/// <returns></returns>
	/// <exception cref="HandledAppException"></exception>
	public Group LoadForLibraryPage(int parentId)
	{
		try
		{
			var loadedGroup = repository.First(
					filter: g => g.Id == parentId,
					includes: query => query
						//Include child groups, their comic books and their information
						.Include(g => g.ChildGroups.OrderBy(cg => cg.Name))
						.ThenInclude(cg => cg.ComicBooks)
						.ThenInclude(cb => cb.Information)

						//Include child groups, their comic books and their pages
						.Include(g => g.ChildGroups.OrderBy(cg => cg.Name))
						.ThenInclude(cg => cg.ComicBooks)
						.ThenInclude(cb => cb.Pages)

						//Include comic books and their information
						.Include(g => g.ComicBooks)
						.ThenInclude(cb => cb.Information)

						//Include comic books and their pages
						.Include(g => g.ComicBooks)
						.ThenInclude(cb => cb.Pages)
				);

			if (loadedGroup == null)
				throw new HandledAppException("Group not found", Severity.Warning);

			loadedGroup.ChildGroups = loadedGroup.ChildGroups
				.Select(LoadCoverImagesForComicBooksInGroupCover)
				.ToList();

			return loadedGroup;
		}
		catch (DataException e)
		{
			throw new HandledAppException("Could not find group by id: " + parentId, Severity.Warning, e);
		}
	}

	public IEnumerable<Group> List()
	{
		return repository.List(filter: g => g.ParentGroupId != null);
	}

	public Group GetById(int groupId)
	{
		var group = repository.GetById(groupId);

		if(group == null)
			throw new HandledAppException("Could not find group by id: " + groupId, Severity.Warning);

		return group;
	}

	public IEnumerable<Group> SearchByName(string searchTerm)
	{
		searchTerm = searchTerm.ToLower();
		logger.Information($"GroupService.SearchByName: Searching for group by name: \"{searchTerm}\"");

		try
		{
			var groups = repository.List(filter: g => g.Name.ToLower().Contains(searchTerm)).ToArray();
			logger.Information($"GroupService.SearchByName: Found {groups.Length} groups for term: \"{searchTerm}\"");
			return groups;
		}
		catch (Exception e)
		{
			throw new HandledAppException("Error loading groups", Severity.Error, e);
		}
	}

	private Group LoadCoverImagesForComicBooksInGroupCover(Group g)
	{
		g.ComicBooks = g.ComicBooks.Take(4).ToList();

		//Check if the cover image exists, and if not, regenerate it.
		foreach (var cb in g.ComicBooks)
		{
			if (File.Exists(cb.Information.SavedCoverImageFullPath))
				continue;

			logger.Information($"GroupService.LoadCoverImagesForComicBooksInGroupCover: Regenerating cover image for: \"{cb.Title}\"");
			var coverImage = cb.Pages.First(page => page.PageType == PageType.Cover);
			new ComicBookImageHandler().SaveThumbnailToDisc(coverImage.PageFileName, cb.FileUri, cb.Id.ToString());
		}

		return g;
	}


	//Create

	public Group Create(string newGroupName, int parentId)
	{
		Group groupToCreate = new()
		{
			Name = newGroupName,
			ParentGroupId = parentId
		};

		repository.Insert(groupToCreate);
		dbContext.SaveChanges();

		return groupToCreate;
	}

	//Update

	public void Rename(int groupId, string newName)
	{
		// groupRepository.Rename(groupId, newName);
		try
		{
			var groupToUpdate = repository.GetById(groupId);

			if(groupToUpdate == null)
				throw new HandledAppException("Could not find group by id: " + groupId, Severity.Warning);

			groupToUpdate.Name = newName;
			repository.Update(groupToUpdate);

			dbContext.SaveChanges();
		}
		catch (Exception)
		{
			throw new HandledAppException("Could not find group by id: " + groupId, Severity.Warning);
		}
	}

	public void AddToGroup(int newParentGroupId, int groupId)
	{
		logger.Information($"GroupService.AddToGroup: Adding group({groupId}) to parent group({newParentGroupId})");
		try
		{
			var groupToUpdate = repository.GetById(groupId);

			if(groupToUpdate == null)
				throw new HandledAppException("Could not find group by id: " + groupId, Severity.Warning);

			groupToUpdate.ParentGroupId = newParentGroupId;
			repository.Update(groupToUpdate);

			dbContext.SaveChanges();
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error adding comic book to group" + groupId, Severity.Error, e);
		}
	}

	//Delete
	public void Delete(int groupId, bool deleteAllContent)
	{
		logger.Information($"GroupService.Delete: GroupId: {groupId}, DeleteAllContent: {deleteAllContent}");

		try
		{
			var currentGroup = repository.First(
				filter: g => g.Id == groupId,
				includes: query => query.Include(g => g.ChildGroups)
			);

			if (currentGroup == null)
				throw new HandledAppException("Could not find group with id: " + groupId, Severity.Warning);

			//Move the group's content to the parent group
			if (!deleteAllContent)
			{
				comicBookService.MoveAll(groupId, currentGroup.ParentGroupId!.Value);
				MoveAll(groupId, currentGroup.ParentGroupId.Value);
			}
			else //Delete everything in the group
			{
				currentGroup
					.ChildGroups
					.ToList()
					.ForEach(cg => Delete(cg.Id, true));

				comicBookService.DeleteAllFromGroup(groupId);
			}

			repository.Delete(currentGroup);
			dbContext.SaveChanges();
		}
		catch (DbUpdateException ex)
		{
			throw new HandledAppException("Error deleting group", Severity.Error, ex);
		}
	}

	private void MoveAll(int currentParentGroupId, int newParentGroupId)
	{
		repository
			.List(filter: g => g.ParentGroupId == currentParentGroupId)
			.Select(g =>
			{
				g.ParentGroupId = newParentGroupId;
				return g;
			})
			.ForEach(repository.Update);

		try
		{
			dbContext.SaveChanges();
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Could not move group content to new group", Severity.Warning, e);
		}
	}
}
