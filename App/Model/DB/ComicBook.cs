using System.ComponentModel.DataAnnotations;

namespace Zine.App.Model.DB;

public class ComicBook
{
	public int Id { get; set; }

	[Required]
	[MaxLength(255)]
	public required string Name { get; set; }

	[Required]
	[MaxLength(255)]
	public required string FileName { get; set; }

	public int? GroupId { get; set; }
	public Group? Group { get; set; }
}
