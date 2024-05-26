using Zine.App.Model.DB;

namespace Zine.App.Repositories;

public interface IGroupRepository
{
	public IEnumerable<Group> GetAllByParentId(int? parentId = null);
	public Group? GetById(int groupId);
	public Group Create(string newGroupName, int? parentId = null);
}