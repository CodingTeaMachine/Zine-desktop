using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zine.App.Domain.ComicBookPageInformation;

[Table("ComicBookPageInformation")]
public class ComicBookPageInformation
{
	public int Id { get; init; }

	[Required]
	[MaxLength(255)]
	public required string PageFileName { get; init; }

	[Required]
	public PageType PageType { get; init; }

	[Required]
	public int ComicBookId { get; init; }
	public ComicBook.ComicBook ComicBook { get; init; } = null!;

	[Required]
	public int PageNumberStart { get; init; }

	[NotMapped]
	public int PageNumberEnd =>
		PageType == PageType.Double
			? PageNumberStart + 1
			: PageNumberStart;
}
