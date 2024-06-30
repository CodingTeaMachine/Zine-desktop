using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zine.App.Domain.ComicBook.FormatHandler;

namespace Zine.App.Domain.ComicBookInformation;

[Table("ComicBookInformation")]
public class ComicBookInformation
{
	public int Id { get; init; }

	public int ComicBookId { get; init; }

	public ComicBook.ComicBook ComicBook { get; init; } = null!;

	public int PageNamingFormat { get; init; }

	[Required]
	[MaxLength(255)]
	public required string CoverImage { get; set; }
	public required string CoverImage { get; init; }
}
