namespace Zine.App.Domain.Group;

public class GroupService(IGroupRepository groupRepository) : IGroupService
{
	public IEnumerable<Group> GetAllByParentId(int? parentId = null)
	{
		// Return only the first 4 comic books from the group
		return groupRepository.GetAllByParentId(parentId)
			.Select(g =>
			{
				g.ComicBooks = g.ComicBooks.Take(4).ToList();
				return g;
			})
			.ToList();
	}

	public Group? GetById(int groupId)
	{
		return groupRepository.GetById(groupId);
	}


	public Group Create(string newGroupName, int? parentId = null)
	{
		return groupRepository.Create(newGroupName, parentId);
	}
}
