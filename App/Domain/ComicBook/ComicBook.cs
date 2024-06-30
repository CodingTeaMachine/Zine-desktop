using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zine.App.Domain.ComicBook;

[Table("ComicBooks")]
public class ComicBook
{
	public int Id { get; init; }

	[Required]
	[MaxLength(255)]
	public required string Name { get; init; }

	[Required]
	[MaxLength(255)]
	public required string FileName { get; init; }

	public int? GroupId { get; set; }

	public Group.Group? Group { get; init; }

	/// <summary>
	///	This field is not stored in the DB, because it can just be calculated at runtime
	/// </summary>
	[NotMapped]
	public ComicBookFormat CompressionFormat
	{
		get
		{
			string fileExtension = Path.GetExtension(FileName);
			return ComicBookFormatFactory.GetFromFileExtension(fileExtension);
		}
	}

}
