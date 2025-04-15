namespace Zine.App.Domain.StatusTag.Converter;

public class StatusTagConverter
{
	public static string ToString(StatusTag? tag)
	{
		return tag?.Name ?? string.Empty;
	}
}
