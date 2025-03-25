using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Common.Service.DTO;
using Zine.App.Database;
using Zine.App.Domain.Publisher.DTO;
using Zine.App.Domain.Tag.DTO;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.Tag;

public class TagService(
	GenericRepository<Tag> repository,
	ZineDbContext dbContext,
	ILoggerService logger) : ITagService
{
	public Tag Create(object obj)
	{
		var dto = (CreateTagDto) obj;
		var publisherToCreate = new Tag { Name = dto.Name };

		repository.Insert(publisherToCreate);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"TagService.Create: Created publisher: '{dto.Name}'");
			return publisherToCreate;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException($"Could not create tag: '{dto.Name}'", Severity.Error, e);
		}

	}

	public IEnumerable<Tag> Search(object searchParams)
	{
		var dto = (GenericSearchDto)searchParams;

		if (dto.SearchTerm == null)
			return repository.List();

		dto.SearchTerm = dto.SearchTerm.ToLower();

		return repository.List(
			filter: t => t.Name.ToLower().Contains(dto.SearchTerm));
	}

	public void Delete(Tag value)
	{

		repository.Delete(value);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"TagService.Delete: Deleted tag: '{value.Name}' ({value.Id})");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException($"Error deleting tag: '{value.Name}'", Severity.Error, e);
		}
	}
}
