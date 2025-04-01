namespace Zine.App.Domain.Tag.Converter;

public class TagConverter
{
	public static string ToString(Tag? tag)
	{
		return tag?.Name ?? string.Empty;
	}
}
