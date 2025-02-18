using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zine.App.Enums;
using Zine.App.Helpers.Compression;

namespace Zine.App.Domain.ComicBookInformation;

[Table("ComicBookInformation")]
public class ComicBookInformation
{
	public int Id { get; init; }

	public int ComicBookId { get; init; }

	/**
	 * It is marked nullable because during the cascade delete the object can lose reference and FileMovedOrDeleted throws an exception
	 */
	public ComicBook.ComicBook? ComicBook { get; init; }

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
			var fileExtension = Path.GetExtension(ComicBook?.FileUri);
			return fileExtension != null ?
				CompressionFormatFactory.GetFromFile(fileExtension)
				: CompressionFormat.Unknown;
		}
	}

	[NotMapped]
	public bool FileMovedOrDeleted => !File.Exists(ComicBook?.FileUri);
}
