using Microsoft.EntityFrameworkCore;
using Zine.App.Database;
using Zine.App.Logger;

namespace Zine.App.Domain.Group;

public class GroupRepository(IDbContextFactory<ZineDbContext> contextFactory, ILoggerService logger)
	: Repository(contextFactory), IGroupRepository
{
	public IEnumerable<Group> GetAllByParentId(int? parentId = null)
	{
		return  GetDbContext().Groups
			.Where(g => g.ParentGroupId == parentId)
			.OrderBy(g => g.Name)
			.Include(g => g.ComicBooks)
			.ThenInclude(cb => cb.Information)
			.ToList();
	}

	public Group? GetById(int groupId)
	{
		return GetDbContext().Groups.FirstOrDefault(g => g.Id == groupId);
	}

	public Group? GetByIdWithChildGroups(int groupId)
	{
		return GetDbContext().Groups.Include(g => g.ChildGroups).FirstOrDefault(g => g.Id == groupId);
	}

	public Group Create(string newGroupName, int? parentId = null)
	{
		var groupToCreate = new Group { Name = newGroupName, ParentGroupId = parentId};
		var context = GetDbContext();
		var createdGroup = context.Groups.Add(groupToCreate);
		context.SaveChanges();
		return createdGroup.Entity;
	}

	public bool Rename(int comicBookId, string newName)
	{
		var group = GetById(comicBookId);
		group!.Name = newName;
		var updatedLines = GetDbContext().SaveChanges();
		return updatedLines == 1;
	}

	public bool AddToGroup(int? newParentGroupId, int groupId)
	{
		var group = GetById(groupId);
		group!.ParentGroupId = newParentGroupId;
		var updatedLines = GetDbContext().SaveChanges();
		return updatedLines == 1;
	}

	public void MoveAll(int? currentParentGroupId, int? newParentGroupId)
	{
		var context = GetDbContext();
		context.Groups
			.Where(g => g.ParentGroupId == currentParentGroupId)
			.ToList()
			.ForEach(g => g.ParentGroupId = newParentGroupId);

		var updatedLines = context.SaveChanges();
		var newParent = newParentGroupId == null ? "root" : newParentGroupId.ToString();
		logger.Information($"ComicBookRepository.MoveAll: Moved {updatedLines} comic books  from: {currentParentGroupId} to: {newParent}");
	}

	public bool Delete(int groupId)
	{
		var context = GetDbContext();
		var group = GetById(groupId);

		if (group == null)
		{
			logger.Warning($"Could not find group by id: {groupId}");
			return false;
		}

		context.Groups.Remove(group);
		var updatedLines = context.SaveChanges();

		return updatedLines == 1;
	}
}
