using System.ComponentModel.DataAnnotations;

namespace Zine.App.Domain.Person;

public class Person
{
	public int Id { get; set; }

	[MaxLength(255)]
	public required string Name { get; set; }

	public Role Role { get; set; }

	public List<ComicBookInformation.ComicBookInformation> ComicBookInformationList { get; } = [];
}
