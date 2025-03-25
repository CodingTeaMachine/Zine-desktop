using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Common.Service.DTO;
using Zine.App.Database;
using Zine.App.Domain.Issue.DTO;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.Issue;

public class IssueService(
	GenericRepository<Issue> repository,
	ZineDbContext dbContext,
	ILoggerService logger) : IIssueService
{
	public Issue Create(object obj)
	{
		var dto = (CreateIssueDto)obj;

		var issueToCreate = new Issue
		{
			Name = dto.Name,
			SeriesId = dto.SeriesId,
		};

		repository.Insert(issueToCreate);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"IssueService.Create: Created issue {issueToCreate.Name}");
			return issueToCreate;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException($"Could not create issue: {issueToCreate.Name}", Severity.Error, e);
		}
	}

	public IEnumerable<Issue> Search(object searchParams)
	{
		var dto = (GenericSearchDto)searchParams;

		if (dto.SearchTerm == null)
			return repository.List();

		dto.SearchTerm = dto.SearchTerm.ToLower();

		return repository.List(
			filter: i => i.Name.ToLower().Contains(dto.SearchTerm));
	}

	public void Delete(Issue value)
	{
		repository.Delete(value);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"IssueService.Delete: Deleted tag: '{value.Name}' ({value.Id})");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException($"Error deleting tag: '{value.Name}'", Severity.Error, e);
		}
	}
}
