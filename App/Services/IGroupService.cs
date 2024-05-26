
using Zine.App.Model.DB;

namespace Zine.App.Services;

public interface IGroupService
{
	public IEnumerable<Group> GetAllByParentId(int? parentId = null);
	public Group? GetById(int groupId);

	public Group Create(string newGroupName, int? parentId = null);
}
