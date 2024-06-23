using System.IO.Compression;
using System.Text.RegularExpressions;
using Zine.App.Domain.ComicBookInformation;

namespace Zine.App.Domain.ComicBook.FormatHandler;

public class GeneralFormatHandler(string filePath, string coverImageDirectory)
{
	public string FilePath { get; set; } = filePath;
	public string CoverImageDirectory { get; set; } = coverImageDirectory;

	private readonly string[] _allowedImageExtensions =
	[
		".bmp",
		".gif",
		".jpg", ".jpeg",
		".png",
		".tif", ".tiff",
		".webp",
	];

	private const string FileNumberingRegex = "((0{2,4})|(0{1,3}1))$";

	private readonly string[] _excludedFirstFileEndings = ["101", "100"];

	protected string GetFilename(ZipArchiveEntry coverImage)
	{
		var filename = coverImage.Name.Replace("#", "_");
		var fileExtension = Path.GetExtension(filename);
		var fileCounter = 1;

		while (Path.Exists(Path.Join(CoverImageDirectory, filename)))
		{
			filename = Path.GetFileNameWithoutExtension(filename) + $"-{fileCounter}" + fileExtension;
			fileCounter++;
		}

		return filename;
	}

	protected bool IsCoverImage(ZipArchiveEntry potentialCoverImage, ComicBookPageNamingFormatName pageNamingFormatName)
	{
		if (!_allowedImageExtensions.Contains(Path.GetExtension(potentialCoverImage.Name)))
			return false;

		var imageName = Path.GetFileNameWithoutExtension(potentialCoverImage.Name).ToLower();

		if (pageNamingFormatName == ComicBookPageNamingFormatName.Enumeration)
		{
			return
				!_excludedFirstFileEndings.Any(excludedFirstFileEnding => imageName.EndsWith(excludedFirstFileEnding))
				&& Regex.IsMatch(imageName, FileNumberingRegex);
		}

		var regex = ComicBookPageNamingFormat.PageFromatToRegexDic[pageNamingFormatName];

		return Regex.IsMatch(imageName, regex);
	}

	protected ComicBookPageNamingFormatName GetPageNamingFormat(ZipArchive comicBookArchive)
	{
		var filenames = comicBookArchive.Entries.Select(comicBookImage => Path.GetFileNameWithoutExtension(comicBookImage.Name)).ToList();

		foreach (var regexToPageFormat in ComicBookPageNamingFormat.PageFromatToRegexDic)
		{
			if (filenames.Any(filename => Regex.IsMatch(filename.ToLower(), regexToPageFormat.Value)))
			{
				return regexToPageFormat.Key;
			}
		}

		return ComicBookPageNamingFormatName.Enumeration;
	}
}
