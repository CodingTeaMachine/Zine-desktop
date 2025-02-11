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

	[Required]
	[MaxLength(25)]
	public string SavedCoverImageFileName { get; init; } = null!;
	
	[NotMapped]
	public string SavedCoverImageFullPath => Path.Join(DataPath.ComicBookCoverDirectory, SavedCoverImageFileName);


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
	public bool FileMovedOrDeleted => !File.Exists(ComicBook.FileUri);
}
