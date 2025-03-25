using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zine.App.Common.FieldInterfaces;

namespace Zine.App.Domain.Group;

/// <summary>
/// The main group is called Library, and is inserted in Migrations\20240914082002_RemoveNullabilityOnParentGroupId.cs
/// </summary>

[Table("Groups")]
public class Group : IId
{
	public int Id { get; init; }

	[Required]
	[MaxLength(255)]
	public required string Name { get; set; }

	public int? ParentGroupId { get; set; } // Only the "Library" group has a null parentId
	public Group? ParentGroup { get; init; } // Only null if the group is the "Library" group

	public ICollection<Group> ChildGroups { get; set; } = [];
	public ICollection<ComicBook.ComicBook> ComicBooks { get; set; } = [];
}
