using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Database;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.Group;

public class GroupRepository(IDbContextFactory<ZineDbContext> contextFactory, ILoggerService logger)
	: Repository(contextFactory), IGroupRepository
{
	///  <summary>
	/// 	Load groups under a specific parent
	///  </summary>
	///  <param name="groupId"></param>
	///  <returns></returns>
	///  <exception cref="HandledAppException">Thrown when the db is not accessible</exception>
	public Group? LoadForLibraryPage(int groupId)
	{
		try
		{
			logger.Information($"Getting all groups for library page by groupId \"{groupId}\"");
			return GetDbContext().Groups
				.Include(g => g.ChildGroups.OrderBy(gOrder => gOrder.Name))
				.ThenInclude(childGroup => childGroup.ComicBooks)
				.ThenInclude(cb => cb.Information)
				.Include(g => g.ComicBooks)
				.ThenInclude(cb => cb.Information)
				.FirstOrDefault(g => g.Id == groupId);
		}
		catch (DbException e)
		{
			throw new HandledAppException("Error loading group", Severity.Error, e);
		}
	}

	public IEnumerable<Group> List()
	{
		return GetDbContext().Groups.Where(g => g.ParentGroupId != null).ToList();
	}

	/// <summary>
	/// Load specific group
	/// </summary>
	/// <param name="groupId"></param>
	/// <returns></returns>
	/// <exception cref="HandledAppException">Thrown when the db is not accessible</exception>
	public Group? GetById(int groupId)
	{
		try
		{
			return GetDbContext().Groups.FirstOrDefault(g => g.Id == groupId);
		}
		catch (DbException e)
		{
			throw new HandledAppException("Error loading group", Severity.Error, e);
		}
	}

	/// <summary>
	/// Load group with child groups
	/// </summary>
	/// <param name="groupId"></param>
	/// <returns></returns>
	/// <exception cref="HandledAppException">Thrown when the db is not accessible</exception>
	public Group? GetByIdWithChildGroups(int groupId)
	{
		try
		{
			return GetDbContext().Groups.Include(g => g.ChildGroups).FirstOrDefault(g => g.Id == groupId);
		}
		catch (DbException e)
		{
			throw new HandledAppException("Error loading group", Severity.Error, e);
		}
	}

	/// <summary>
	/// Create a new group
	/// </summary>
	/// <param name="newGroupName"></param>
	/// <param name="parentId"></param>
	/// <returns></returns>
	/// <exception cref="HandledAppException">Thrown when the group could not be created</exception>
	public Group Create(string newGroupName, int parentId)
	{
		var groupToCreate = new Group { Name = newGroupName, ParentGroupId = parentId};
		try
		{
			var context = GetDbContext();
			var createdGroup = context.Groups.Add(groupToCreate);
			context.SaveChanges();
			return createdGroup.Entity;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error creating group", Severity.Error, e);
		}
		finally
		{
			DisposeDbContext();
		}

	}

	/// <summary>
	/// Renames a specific group
	/// </summary>
	/// <param name="groupId"></param>
	/// <param name="newName"></param>
	/// <returns></returns>
	/// <exception cref="HandledAppException">Thrown when the group could not be renamed</exception>
	public bool Rename(int groupId, string newName)
	{
		var group = GetById(groupId);
		var oldGroupName = group!.Name;
		group.Name = newName;
		try
		{
			var updatedLines = GetDbContext().SaveChanges();
			var updateSuccessful = updatedLines == 1;

			if (updateSuccessful)
				logger.Information(
					$"GroupRepository.Rename: Renamed group from: {oldGroupName} to: {newName} (id: {groupId})");
			else
				throw new DbUpdateException($"Error renaming group from: {oldGroupName} to: {newName} ({groupId} updatedLines:{updatedLines})");

			return updateSuccessful;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Could not rename group", Severity.Warning, e);
		}
		finally
		{
			DisposeDbContext();
		}
	}

	/// <summary>
	/// Moves group into another group
	/// </summary>
	/// <param name="newParentGroupId"></param>
	/// <param name="groupId"></param>
	/// <returns></returns>
	/// <exception cref="HandledAppException">Thrown when the group could not be moved</exception>
	public bool AddToGroup(int newParentGroupId, int groupId)
	{
		var group = GetById(groupId);
		group!.ParentGroupId = newParentGroupId;
		try
		{
			var updatedLines = GetDbContext().SaveChanges();
			return updatedLines == 1;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Could not add to group", Severity.Warning, e);
		}
		finally
		{
			DisposeDbContext();
		}
	}

	/// <summary>
	/// Moves everything from one group to another
	/// </summary>
	/// <param name="currentParentGroupId"></param>
	/// <param name="newParentGroupId"></param>
	/// <exception cref="HandledAppException">Thrown when the content of the group could not be moved</exception>
	public void MoveAll(int currentParentGroupId, int newParentGroupId)
	{
		var context = GetDbContext();
		context.Groups
			.Where(g => g.ParentGroupId == currentParentGroupId)
			.ToList()
			.ForEach(g => g.ParentGroupId = newParentGroupId);

		try
		{
			var updatedLines = context.SaveChanges();
			logger.Information(
				$"ComicBookRepository.MoveAll: Moved {updatedLines} comic books  from: {currentParentGroupId} to: {newParentGroupId.ToString()}");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Could not move group content to new group", Severity.Warning, e);
		}
		finally
		{
			DisposeDbContext();
		}
	}

	/// <summary>
	/// Delete a group
	/// </summary>
	/// <param name="groupId"></param>
	/// <returns></returns>
	/// <exception cref="HandledAppException">Thrown when the group could not be deleted</exception>
	public bool Delete(int groupId)
	{
		var group = GetById(groupId)!;

		try
		{
			var context = GetDbContext();
			context.Groups.Remove(group);
			var updatedLines = context.SaveChanges();
			return updatedLines == 1; // TODO: Itt hibát kell dobni, ha nem 1 az updated lines
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Could not delete group", Severity.Error, e);
		}
		finally
		{
			DisposeDbContext();
		}
	}

	public IEnumerable<Group> SearchByName(string searchTerm)
	{
		searchTerm = searchTerm.ToLower();
		logger.Information($"GroupRepository.SearchByName: Searching for group by name: \"{searchTerm}\"");

		try
		{
			var context = GetDbContext();
			var groupsFoundByName = context.Groups
				.Where(cb => cb.Name.ToLower().Contains(searchTerm));
			logger.Information($"GroupRepository.SearchByName: Found {groupsFoundByName.Count()} groups for term: \"{searchTerm}\"");

			return groupsFoundByName;
		}
		catch (Exception e)
		{
			throw new HandledAppException($"Error searching groups by name: \"{searchTerm}\"", Severity.Error, e);
		}
		finally
		{
			DisposeDbContext();
		}
	}
}
