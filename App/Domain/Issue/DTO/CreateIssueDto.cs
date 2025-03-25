namespace Zine.App.Domain.Issue.DTO;

public class CreateIssueDto
{
	public required string Name { get; set; }
	public required int SeriesId { get; set; }
}
