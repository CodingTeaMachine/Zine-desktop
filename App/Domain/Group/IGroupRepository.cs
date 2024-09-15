namespace Zine.App.Domain.Group;

public interface IGroupRepository
{
	public Group? LoadForLibraryPage(int groupId);
	public Group? GetByIdWithChildGroups(int groupId);


	public Group? GetById(int groupId);
	public Group Create(string newGroupName, int parentId);
	public bool Rename(int groupId, string newName);
	public bool AddToGroup(int newParentGroupId, int groupId);
	public void MoveAll(int currentParentGroupId, int newParentGroupId);
	public bool Delete(int groupId);
}
