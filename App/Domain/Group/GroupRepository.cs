using Microsoft.EntityFrameworkCore;
using Zine.App.Database;

namespace Zine.App.Domain.Group;

public class GroupRepository(IDbContextFactory<ZineDbContext> contextFactory)
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

	public Group Create(string newGroupName, int? parentId = null)
	{
		var groupToCreate = new Group { Name = newGroupName, ParentGroupId = parentId};
		var context = GetDbContext();
		var createdGroup = context.Groups.Add(groupToCreate);
		context.SaveChanges();
		return createdGroup.Entity;
	}
}
