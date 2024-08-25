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
	/// <summary>
	///	Load groups under a specific parent
	/// </summary>
	/// <param name="parentId"></param>
	/// <returns></returns>
	/// <exception cref="HandledAppException">Thrown when the db is not accessible</exception>
	public IEnumerable<Group> GetAllByParentId(int? parentId = null)
	{
		try
		{
			return GetDbContext().Groups
				.Where(g => g.ParentGroupId == parentId)
				.OrderBy(g => g.Name)
				.Include(g => g.ComicBooks)
				.ThenInclude(cb => cb.Information)
				.ToList();
		}
		catch (DbException e)
		{
			throw new HandledAppException("Error loading group", Severity.Error, e);
		}

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
		catch(DbException e)
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
	public Group Create(string newGroupName, int? parentId = null)
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
			throw new HandledAppException(e.Message, Severity.Error, e);
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
		group!.Name = newName;
		
		try
		{
			var updatedLines = GetDbContext().SaveChanges();
			return updatedLines == 1;
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
	public bool AddToGroup(int? newParentGroupId, int groupId)
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
	}

	/// <summary>
	/// Moves everything from one group to another
	/// </summary>
	/// <param name="currentParentGroupId"></param>
	/// <param name="newParentGroupId"></param>
	/// <exception cref="HandledAppException">Thrown when the content of the group could not be moved</exception>
	public void MoveAll(int? currentParentGroupId, int? newParentGroupId)
	{
		var context = GetDbContext();
		context.Groups
			.Where(g => g.ParentGroupId == currentParentGroupId)
			.ToList()
			.ForEach(g => g.ParentGroupId = newParentGroupId);

		try
		{
			var updatedLines = context.SaveChanges();
			var newParent = newParentGroupId == null ? "root" : newParentGroupId.ToString();
			logger.Information($"ComicBookRepository.MoveAll: Moved {updatedLines} comic books  from: {currentParentGroupId} to: {newParent}");
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
	}
}
