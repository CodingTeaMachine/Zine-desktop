using Zine.App.Domain.Group.DTO;

namespace Zine.App.Domain.Group;

public interface IGroupService
{
	public IEnumerable<Group> Search(GroupSearchDto search);
	public Group? LoadForLibraryPage(int parentId);


	public IEnumerable<Group> List();
	public Group GetById(int groupId);
	public Group Create(string newGroupName, int parentId);
	public void Rename(int groupId, string newName);
	public void AddToGroup(int newParentGroupId, int groupId);
	public void Delete(int groupId, bool deleteAllContent);
}
