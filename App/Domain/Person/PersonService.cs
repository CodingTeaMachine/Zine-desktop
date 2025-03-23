using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Database;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.Person;

public class PersonService(GenericRepository<Person> repository, ZineDbContext dbContext, ILoggerService logger) : IPersonService
{
	public Person Create(string name, Role role)
	{
		var person = new Person
		{
			Name = name,
			Role = role,
		};

		repository.Insert(person);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"PersonService.Create: Created person: \"{name}\" with role: {role.ToString()}");
			return person;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException($"Failed to create person: \"{name}\"", Severity.Error, e);
		}
	}

	public IEnumerable<Person> Search(string? name, Role role)
	{
		if (string.IsNullOrEmpty(name))
		{
			return repository.List(filter: p => p.Role == role);
		}
		name = name.ToLower();
		return repository.List(
			filter: p => p.Role == role && p.Name.ToLower().Contains(name));
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
