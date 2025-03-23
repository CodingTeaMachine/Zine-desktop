namespace Zine.App.Domain.Person;

public interface IPersonService
{
	public Person Create(string name, Role role);

	public IEnumerable<Person> Search(string? name, Role role);

	public void Delete(Person person);
}
