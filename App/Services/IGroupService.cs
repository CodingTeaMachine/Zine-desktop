
using Zine.App.Model.DB;

namespace Zine.App.Services;

public interface IGroupService
{
	public IEnumerable<Group> GetAllByParentId(int? parentId = null);
}
