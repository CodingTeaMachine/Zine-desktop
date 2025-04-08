using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zine.App.Common.FieldInterfaces;
using Zine.App.Domain.Person;
using Zine.App.Enums;
using Zine.App.Helpers.Compression;

namespace Zine.App.Domain.ComicBookInformation;

[Table("ComicBookInformation")]
public class ComicBookInformation : IId
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

	public DateTime? LastOpened { get; set; }

	public DateTime? ReleaseDate { get; set; }

	public List<Person.Person> People { get; set; } = [];

	public List<Publisher.Publisher> Publishers { get; set; } = [];

	public List<Tag.Tag> Tags { get; set; } = [];

	public Series.Series? Series { get; set; }

	[MaxLength(50)]
	public string Issue { get; set; } = String.Empty;

	// Creator type shortcuts

	[NotMapped]
	public List<Person.Person> Draftsmen => People.Where(p => p.Role == Role.Drawer).ToList();

	[NotMapped]
	public List<Person.Person> Colorists => People.Where(p => p.Role == Role.Colorist).ToList();

	[NotMapped]
	public List<Person.Person> Writers => People.Where(p => p.Role == Role.Writer).ToList();

	[NotMapped]
	public List<Person.Person> Editors => People.Where(p => p.Role == Role.Editor).ToList();

	//

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
