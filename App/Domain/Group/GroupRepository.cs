using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Database;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.Group;

public class GroupRepository(IDbContextFactory<ZineDbContext> contextFactory, ILoggerService logger)
	: IGroupRepository
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
			using var context = contextFactory.CreateDbContext();
			logger.Information($"Getting all groups for library page by groupId \"{groupId}\"");
			return context.Groups
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
		using var context = contextFactory.CreateDbContext();
		return context.Groups.Where(g => g.ParentGroupId != null).ToList();
	}

	/// <summary>
	/// Load specific group
	/// </summary>
	/// <param name="groupId"></param>
	/// <returns></returns>
	/// <exception cref="HandledAppException">Thrown when the db is not accessible</exception>
	public Group GetById(int groupId)
	{
		try
		{
			using var context = contextFactory.CreateDbContext();
			var group = context.Groups.FirstOrDefault(g => g.Id == groupId);

			if (group == null)
				throw new HandledAppException("Group does not exist", Severity.Warning);

			return group;

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
			using var context = contextFactory.CreateDbContext();
			return context.Groups.Include(g => g.ChildGroups).FirstOrDefault(g => g.Id == groupId);
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
			using var context = contextFactory.CreateDbContext();
			var createdGroup = context.Groups.Add(groupToCreate);
			context.SaveChanges();
			return createdGroup.Entity;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error creating group", Severity.Error, e);
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
			using var context = contextFactory.CreateDbContext();
			var updatedLines = context.SaveChanges();
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
			using var context = contextFactory.CreateDbContext();
			var updatedLines = context.SaveChanges();
			return updatedLines == 1;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Could not add to group", Severity.Warning, e);
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
		var context = contextFactory.CreateDbContext();
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
	}

	/// <summary>
	/// Delete a group
	/// </summary>
	/// <param name="groupId"></param>
	/// <returns></returns>
	/// <exception cref="HandledAppException">Thrown when the group could not be deleted</exception>
	public bool Delete(int groupId)
	{
		try
		{
			var groupToDelete = GetById(groupId);

			using var context = contextFactory.CreateDbContext();
			context.Groups.Attach(groupToDelete);
			context.Groups.Remove(groupToDelete);
			var updatedLines = context.SaveChanges();

			return updatedLines == 1; // TODO: Itt hibát kell dobni, ha nem 1 az updated lines
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Could not delete group", Severity.Error, e);
		}
	}

	public IEnumerable<Group> SearchByName(string searchTerm)
	{
		searchTerm = searchTerm.ToLower();
		logger.Information($"GroupRepository.SearchByName: Searching for group by name: \"{searchTerm}\"");

		try
		{
			using var context = contextFactory.CreateDbContext();
			var groupsFoundByName = context.Groups
				.Where(cb => cb.Name.ToLower().Contains(searchTerm));
			logger.Information($"GroupRepository.SearchByName: Found {groupsFoundByName.Count()} groups for term: \"{searchTerm}\"");

			return groupsFoundByName;
		}
		catch (Exception e)
		{
			throw new HandledAppException($"Error searching groups by name: \"{searchTerm}\"", Severity.Error, e);
		}
	}
}
