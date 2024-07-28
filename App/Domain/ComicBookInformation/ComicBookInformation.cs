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
	public required string CoverImage { get; init; }

	/// <summary>
	///	This field is not stored in the DB, because it can just be calculated at runtime
	/// </summary>
	[NotMapped]
	public ComicBookFormat CompressionFormat
	{
		get
		{
			var fileExtension = Path.GetExtension(ComicBook.FileUri);
			return ComicBookFormatFactory.GetFromFileExtension(fileExtension);
		}
	}

	[NotMapped]
	public bool MovedOrDeleted => !File.Exists(ComicBook.FileUri);
}
