using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Common.Service.DTO;
using Zine.App.Database;
using Zine.App.Domain.Series.DTO;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.Series;

public class SeriesService(
	GenericRepository<Series> repository,
	ZineDbContext dbContext,
	ILoggerService logger) : ISeriesService
{
	public Series Create(object obj)
	{
		var dto = (CreateSeriesDto) obj;
		var seriesToCreate = new Series { Name = dto.Name };

		repository.Insert(seriesToCreate);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"SeriesService.Create: Created series: '{dto.Name}'");
			return seriesToCreate;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException($"Could not create series: '{dto.Name}'", Severity.Error, e);
		}

	}

	public IEnumerable<Series> Search(object searchParams)
	{
		var dto = (GenericSearchDto)searchParams;

		if (dto.SearchTerm == null)
			return repository.List();

		dto.SearchTerm = dto.SearchTerm.ToLower();

		return repository.List(
			filter: s => s.Name.ToLower().Contains(dto.SearchTerm));
	}

	public void Delete(Series value)
	{

		repository.Delete(value);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"SeriesService.Delete: Deleted series: '{value.Name}' ({value.Id})");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException($"Error deleting series: '{value.Name}'", Severity.Error, e);
		}
	}
}
