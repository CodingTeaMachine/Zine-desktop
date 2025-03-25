using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Common.Service.DTO;
using Zine.App.Database;
using Zine.App.Domain.Publisher.DTO;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.Publisher;

public class PublisherService(
	GenericRepository<Publisher> repository,
	ZineDbContext dbContext,
	ILoggerService logger) : IPublisherService
{
	public Publisher Create(object obj)
	{
		var dto = (CreatePublisherDto) obj;
		var publisherToCreate = new Publisher { Name = dto.Name };

		repository.Insert(publisherToCreate);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"PublisherService.Create: Created publisher: '{dto.Name}'");
			return publisherToCreate;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException($"Could not create publisher: '{dto.Name}'", Severity.Error, e);
		}

	}

	public IEnumerable<Publisher> Search(object searchParams)
	{
		var dto = (GenericSearchDto)searchParams;

		if (dto.SearchTerm == null)
			return repository.List();

		dto.SearchTerm = dto.SearchTerm.ToLower();

		return repository.List(
			filter: p => p.Name.ToLower().Contains(dto.SearchTerm));
	}

	public void Delete(Publisher value)
	{

		repository.Delete(value);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"PublisherService.Delete: Deleted publisher: '{value.Name}' ({value.Id})");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException($"Error deleting publisher: '{value.Name}'", Severity.Error, e);
		}
	}
}
