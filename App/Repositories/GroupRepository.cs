using Microsoft.EntityFrameworkCore;
using Zine.App.Database;
using Zine.App.Model.DB;

namespace Zine.App.Repositories;

public class GroupRepository(IDbContextFactory<ZineDbContext> contextFactory)
	: Repository(contextFactory), IGroupRepository
{
	public IEnumerable<Group> GetAllByParentId(int? parentId = null)
	{
		return  GetDbContext().Groups
			.Where(g => g.ParentGroupId == parentId)
			.OrderBy(g => g.Name)
			.Include(g => g.ComicBooks)
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