namespace Zine.App.Domain.Person.Converter;

public class PersonConvert
{
	public static string ToString(Person? person)
	{
		return person?.Name ?? string.Empty;
	}
}
