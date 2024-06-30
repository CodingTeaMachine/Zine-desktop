using Zine.App.Enums;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.Group;

public class GroupService(IGroupRepository groupRepository, ILoggerService logger) : IGroupService
{
	public IEnumerable<Group> GetAllByParentId(int? parentId = null)
	{
		// Return only the first 4 comic books from the group
		return groupRepository.GetAllByParentId(parentId)
			.Select(g =>
			{
				g.ComicBooks = g.ComicBooks.Take(4).ToList();

				//Check if the cover image exists, and if not, regenerate it.
				foreach (var cb in g.ComicBooks)
				{
					if (!File.Exists(Path.Join(DataPath.ComicBookCoverDirectory, cb.Information.CoverImage)))
					{
						logger.Warning($"Regenerating cover image for: {cb.Name}");
						new ComicBookInformationFactory().GetCoverImage(Path.Join(DataPath.ComicBookLinkDirectory, cb.FileName),
							cb.Information.CompressionFormat);
					}
				}

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

	public bool Rename(int groupId, string newName)
	{
		return groupRepository.Rename(groupId, newName);
	}
}
