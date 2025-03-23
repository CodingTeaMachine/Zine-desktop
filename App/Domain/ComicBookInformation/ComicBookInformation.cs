using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zine.App.Domain.Person;
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


	public DateTime? LastOpened { get; set; }

	public ICollection<Person.Person> People { get; set; } = [];

	[NotMapped]
	public ICollection<Person.Person> Draftsmen => People.Where(p => p.Role == Role.Drawer).ToArray();

	[NotMapped]
	public ICollection<Person.Person> Colorists => People.Where(p => p.Role == Role.Colorist).ToArray();

	[NotMapped]
	public ICollection<Person.Person> Writers => People.Where(p => p.Role == Role.Writer).ToArray();

	[NotMapped]
	public ICollection<Person.Person> Editors => People.Where(p => p.Role == Role.Editor).ToArray();




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
