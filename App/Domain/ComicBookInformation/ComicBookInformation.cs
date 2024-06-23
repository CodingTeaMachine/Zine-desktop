using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zine.App.Domain.ComicBookInformation;

[Table("ComicBookInformation")]
public class ComicBookInformation
{
	public int Id { get; set; }

	public int ComicBookId { get; set; }

	public ComicBook.ComicBook ComicBook { get; set; }

	public int PageFormat { get; set; }

	[Required]
	[MaxLength(255)]
	public required string CoverImage { get; set; }
}
