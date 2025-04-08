using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Database;
using Zine.App.Domain.Person.DTO;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.Person;

public class PersonService(
	GenericRepository<Person> repository,
	ZineDbContext dbContext,
	ILoggerService logger) : IPersonService
{
	public Person Create(object obj)
	{
		var dto = (CreatePersonDto)obj;

		var person = new Person
		{
			Name = dto.Name,
			Role = dto.Role,
		};

		repository.Insert(person);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"PersonService.Create: Created person: \"{dto.Name}\" with role: {dto.Role.ToString()}");
			return person;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException($"Failed to create person: \"{dto.Name}\"", Severity.Error, e);
		}
	}

	public IEnumerable<Person> Search(object searchParams)
	{

		var dto = (SearchPersonDto)searchParams;

		if (string.IsNullOrEmpty(dto.Name))
		{
			return repository.List(filter: p => p.Role == dto.Role);
		}

		dto.Name = dto.Name.ToLower();
		return repository.List(
			filter: p => p.Role == dto.Role && p.Name.ToLower().Contains(dto.Name));
	}

	public void Delete(Person person)
	{
		repository.Delete(person);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"PersonService.Delete: Deleted person: \"{person.Name}\" ({person.Id})");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException($"Failed to delete person: \"{person.Name}\"", Severity.Error, e);
		}
	}
}
