namespace Zine.App.Domain.Group;

public interface IGroupService
{
	public Group? LoadForLibraryPage(int parentId);
	public Group? GetById(int groupId);
	public Group Create(string newGroupName, int parentId);
	public bool Rename(int groupId, string newName);
	public bool AddToGroup(int newParentGroupId, int groupId);
	public void MoveAll(int currentParentGroupId, int newParentGroupId);
	public bool Delete(int groupId, bool deleteAllContent);

	//More specific ones
	public string GetName(int groupId);
}
