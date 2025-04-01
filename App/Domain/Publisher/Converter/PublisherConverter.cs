namespace Zine.App.Domain.Publisher.Converter;

public class PublisherConverter
{
	public static string ToString(Publisher? publisher)
	{
		return publisher?.Name ?? string.Empty;
	}
}
