using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zine.App.Common.FieldInterfaces;

namespace Zine.App.Domain.Person;

[Table("People")]
public class Person : IId
{
	public int Id { get; init; }

	[MaxLength(255)]
	public required string Name { get; set; }

	public Role Role { get; set; }

	public List<ComicBookInformation.ComicBookInformation> ComicBookInformationList { get; } = [];
}
