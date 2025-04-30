namespace Zine.App.Enums;

public static class DataPath
{
	public const string ComicBookCoverDirectory = "wwwroot/images/ComicBookCovers";
	public static string ComicBookReadingDirectoryFromAssetRoot => Path.Combine("images", "Reading");
	public static string ComicBookReadingDirectory => Path.Combine(Environment.CurrentDirectory, "wwwroot", ComicBookReadingDirectoryFromAssetRoot);
}
