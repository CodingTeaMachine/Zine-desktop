using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zine.App.Domain.Group;

[Table("Groups")]
public class Group
{
	public int Id { get; init; }

	[Required]
	[MaxLength(255)]
	public required string Name { get; set; }

	public int? ParentGroupId { get; set; }
	public Group? ParentGroup { get; init; }

	public ICollection<Group> ChildGroups { get; set; } = [];
	public ICollection<ComicBook.ComicBook> ComicBooks { get; set; } = [];
}
