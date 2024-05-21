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
			.Include(g => g.ComicBooks)
			.ToList();
	}
}
