using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Zine.App.Common.FieldInterfaces;

namespace Zine.App.Domain.Series;

[Table("Series")]
public class Series : IId
{
	public int Id { get; init; }

	[MaxLength(255)]
	public required string Name { get; set; }

	public List<ComicBookInformation.ComicBookInformation> ComicBookInformationList { get; set; } = [];
}
