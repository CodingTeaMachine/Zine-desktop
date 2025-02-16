namespace Zine.App.Domain.Group;

public interface IGroupService
{
	public IEnumerable<Group> SearchByName(string searchTerm);
	public Group? LoadForLibraryPage(int parentId);


	public IEnumerable<Group> List();
	public Group GetById(int groupId);
	public Group Create(string newGroupName, int parentId);
	public void Rename(int groupId, string newName);
	public void AddToGroup(int newParentGroupId, int groupId);
	public void Delete(int groupId, bool deleteAllContent);
}
