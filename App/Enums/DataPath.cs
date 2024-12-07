namespace Zine.App.Enums;

public static class DataPath
{
	public const string ComicBookCoverDirectory = "wwwroot/images/ComicBookCovers";
	public static string ComicBookReadingDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempReading");
}
