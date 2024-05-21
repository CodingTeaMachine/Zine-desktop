using Zine.App.Model.DB;

namespace Zine.App.Repositories;

public interface IGroupRepository
{
	public IEnumerable<Group> GetAllByParentId(int? parentId = null);
}
