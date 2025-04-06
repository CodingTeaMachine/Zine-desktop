using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zine.App.Common.FieldInterfaces;

namespace Zine.App.Domain.ComicBookPageInformation;

[Table("ComicBookPageInformation")]
public class ComicBookPageInformation : IId
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
	public int Index { get; set; }

	[DefaultValue(false)]
	public bool IsWidthChecked { get; set; }

	[DefaultValue(false)]
	public bool IsRead { get; set; }
}
