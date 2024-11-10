using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;
using Zine.App.Enums;

namespace Zine.App.Domain.ComicBookInformation;

[Table("ComicBookInformation")]
public class ComicBookInformation
{
	public int Id { get; init; }

	public int ComicBookId { get; init; }

	public ComicBook.ComicBook ComicBook { get; init; } = null!;

	public int PageNamingFormat { get; init; }

	public int NumberOfPages { get; init; }

	[Required]
	[MaxLength(255)]
	public required string CoverImage { get; init; }

	[NotMapped]
	public string CoverImageFullPath => Path.Join(DataPath.ComicBookCoverDirectory, CoverImage);

	/// <summary>
	///	This field is not stored in the DB, because it can just be calculated at runtime
	/// </summary>
	[NotMapped]
	public CompressionFormat CompressionCompressionFormat
	{
		get
		{
			var fileExtension = Path.GetExtension(ComicBook.FileUri);
			return CompressionFormatFactory.GetFromFile(fileExtension);
		}
	}

	[NotMapped]
	public bool MovedOrDeleted => !File.Exists(ComicBook.FileUri);
}
