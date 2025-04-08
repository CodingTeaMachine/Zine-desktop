using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zine.App.Common.FieldInterfaces;

namespace Zine.App.Domain.ComicBook;

[Table("ComicBooks")]
public class ComicBook : IId
{
	public int Id { get; init; }

	[Required]
	[MaxLength(255)]
	public required string Title { get; set; }

	/**
	 * Max path length limit per os:
	 *
	 * Windows: 32_767
	 * OSX / Linux: 4096
	 *
	 */
	[Required]
	[MaxLength(32_767)]
	public required string FileUri { get; init; }

	public int GroupId { get; set; }

	public Group.Group Group { get; set; } = null!;

	public ComicBookInformation.ComicBookInformation Information { get; init; } = null!;

	public ICollection<ComicBookPageInformation.ComicBookPageInformation> Pages { get; init; } = null!;

	[NotMapped]
	public int ReadPercentage
	{
		get {
			var readPages = Pages.Count(p => p.IsRead);
			var readPageRatio = readPages / (double)Pages.Count;
			return (int)Math.Round(readPageRatio * 100, 0);
		}
	}
}
