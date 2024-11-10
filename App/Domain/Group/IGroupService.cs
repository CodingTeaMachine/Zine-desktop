namespace Zine.App.Domain.Group;

public interface IGroupService
{
	public string GetName(int groupId);
	public IEnumerable<Group> SearchByName(string searchTerm);
	public Group? LoadForLibraryPage(int parentId);


	public IEnumerable<Group> List();
	public Group? GetById(int groupId);
	public Group Create(string newGroupName, int parentId);
	public bool Rename(int groupId, string newName);
	public bool AddToGroup(int newParentGroupId, int groupId);
	public void MoveAll(int currentParentGroupId, int newParentGroupId);
	public bool Delete(int groupId, bool deleteAllContent);
}
