using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zine.App.Common.FieldInterfaces;

namespace Zine.App.Domain.Tag;

[Table("Tags")]
public class Tag : IId
{
	public int Id { get; init; }

	[MaxLength(255)]
	public required string Name { get; init; }

	public List<ComicBookInformation.ComicBookInformation> ComicBookInformationList { get; } = [];
}
