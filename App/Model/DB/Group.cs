using System.ComponentModel.DataAnnotations;

namespace Zine.App.Model.DB;

public class Group
{
	public int Id { get; set; }

	[Required]
	[MaxLength(255)]
	public required string Name { get; set; }

	public int? ParentGroupId { get; set; }
	public Group? ParentGroup { get; set; }

	public ICollection<Group> ChildGroups { get; set; }
	public ICollection<ComicBook> ComicBooks { get; set; }
}
