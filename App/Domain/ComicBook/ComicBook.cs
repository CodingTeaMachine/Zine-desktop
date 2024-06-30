using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zine.App.Domain.ComicBook;

[Table("ComicBooks")]
public class ComicBook
{
	public int Id { get; init; }

	[Required]
	[MaxLength(255)]
	public required string Name { get; init; }

	[Required]
	[MaxLength(255)]
	public required string FileName { get; init; }

	public int? GroupId { get; set; }

	public Group.Group? Group { get; init; }

	public ComicBookInformation.ComicBookInformation Information { get; init; } = null!;
}
