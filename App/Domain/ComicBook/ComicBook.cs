using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zine.App.Domain.ComicBook.FormatHandler;

namespace Zine.App.Domain.ComicBook;

[Table("ComicBooks")]
public class ComicBook
{
	public int Id { get; set; }

	[Required]
	[MaxLength(255)]
	public required string Name { get; set; }

	[Required]
	[MaxLength(255)]
	public required string FileName { get; set; }

	public int? GroupId { get; set; }

	public Group.Group? Group { get; set; }

	public ComicBookInformation.ComicBookInformation Information { get; set; }

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
