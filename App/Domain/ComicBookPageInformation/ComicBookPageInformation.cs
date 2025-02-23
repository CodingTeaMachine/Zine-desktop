using System.ComponentModel;
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
	public PageType PageType { get; set; }

	[Required]
	public int ComicBookId { get; init; }
	public ComicBook.ComicBook ComicBook { get; init; } = null!;

	[Required]
	public int PageNumberStart { get; set; }

	[DefaultValue(false)]
	public bool IsWidthChecked { get; set; }

	[NotMapped]
	public int PageNumberEnd =>
		PageType == PageType.Double
			? PageNumberStart + 1
			: PageNumberStart;
}
