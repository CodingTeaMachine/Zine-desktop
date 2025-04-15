using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MudBlazor;
using Zine.App.Common.FieldInterfaces;

namespace Zine.App.Domain.StatusTag;

[Table("StatusTags")]
public class StatusTag : IId
{
	public int Id { get; init; }

	[MaxLength(255)]
	public required string Name { get; init; }

	public required Color Color { get; init; }

	public List<ComicBookInformation.ComicBookInformation> ComicBookInformationList { get; set; } = [];
}
