namespace Zine.App.Domain.Series.Converter;

public class SeriesConverter
{
	public static string ToString(Series? series)
	{
		return series?.Name ?? string.Empty;
	}
}
