using System.ComponentModel.DataAnnotations;
using Zine.App.Database.FieldInterfaces;

namespace Zine.App.Domain.Person;

public class Person : IId
{
	public int Id { get; init; }

	[MaxLength(255)]
	public required string Name { get; set; }

	public Role Role { get; set; }

	public List<ComicBookInformation.ComicBookInformation> ComicBookInformationList { get; } = [];
}
