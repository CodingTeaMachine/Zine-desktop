namespace Zine.App.Domain.Person.DTO;

public class CreatePersonDto
{
	public required string Name { get; set; }
	public Role Role { get; set; }
}
