namespace Zine.App.Domain.Group;

public interface IGroupRepository
{
	public IEnumerable<Group> GetAllByParentId(int? parentId = null);
	public Group? GetById(int groupId);
	public Group Create(string newGroupName, int? parentId = null);
}
