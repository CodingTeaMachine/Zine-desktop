using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zine.App.Common.FieldInterfaces;

namespace Zine.App.Domain.Issue;

[Table("Issues")]
public class Issue : IId
{
	public int Id { get; init; }

	[MaxLength(255)]
	public required string Name { get; init; }

	public int SeriesId { get; set; }
	public Series.Series Series { get; set; } = null!;

	public List<ComicBookInformation.ComicBookInformation> ComicBookInformationList { get; set; } = [];
}
