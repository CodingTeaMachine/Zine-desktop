using System.Data;
using SharpCompress.Archives;
using Zine.App.Helpers;
using static System.Text.RegularExpressions.Regex;

namespace Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;

public partial class CompressedFileHandler(string filePath, string outputCoverImageDirectory)
{
	private const string FileNumberingRegex = "((0{2,4})|(0{1,3}1))$";

	[System.Text.RegularExpressions.GeneratedRegex(FileNumberingRegex)]
	private static partial System.Text.RegularExpressions.Regex PageFormatRegex();

	private readonly string[] _excludedFirstFileEndings = ["101", "100"];

	public string ExtractCoverImage(string outputFileName)
	{
		using IArchive comicBookFile = ArchiveFactory.Open(filePath);
		if (!comicBookFile.Entries.Any()) throw new DataException("Empty comic book file");


		ComicBookPageNamingFormatName namingFormatName = GetPageNamingFormat(comicBookFile);
		IArchiveEntry coverImage = comicBookFile.Entries.First(cI => IsCoverImage(cI, namingFormatName));

		var coverImageExtenstion = Path.GetExtension(coverImage.Key!);
		var outputFileNameWithExtension = outputFileName + coverImageExtenstion;
		var outputPath = Path.Join(outputCoverImageDirectory, outputFileNameWithExtension);

		try
		{
			var resizedImageData = Image.GetResizedImage(coverImage, 369, 240);
			File.WriteAllBytes(outputPath, resizedImageData);
		}
		catch (Exception ex) when (ex is NotSupportedException or DataException)
		{
			//Simply save the original image
			coverImage.WriteToFile(outputPath);
		}

		return outputFileNameWithExtension;
	}

	private bool IsCoverImage(IArchiveEntry potentialCoverImage, ComicBookPageNamingFormatName pageNamingFormatName)
	{
		if (!Image.DotExtensions.Contains(Path.GetExtension(potentialCoverImage.Key)))
			return false;

		var imageName = Path.GetFileNameWithoutExtension(potentialCoverImage.Key)?.ToLower();

		if (pageNamingFormatName == ComicBookPageNamingFormatName.Enumeration)
		{
			return
				!_excludedFirstFileEndings.Any(excludedFirstFileEnding => imageName!.EndsWith(excludedFirstFileEnding))
				&& PageFormatRegex().IsMatch(imageName!);
		}

		var regex = ComicBookPageNamingFormat.PageFormatToRegexDic[pageNamingFormatName];

		return IsMatch(imageName!, regex);
	}

	private static ComicBookPageNamingFormatName GetPageNamingFormat(IArchive comicBookArchive)
	{
		var filenames = comicBookArchive.Entries.Select(comicBookImage => Path.GetFileNameWithoutExtension(comicBookImage.Key)).ToList();

		try
		{
			var regexToPageFormat =
				ComicBookPageNamingFormat
					.PageFormatToRegexDic
					.First(regexToPageFormat =>
						filenames.Any(filename => IsMatch(filename!, regexToPageFormat.Value)));

			return regexToPageFormat.Key;
		}
		catch (InvalidOperationException)
		{
			return ComicBookPageNamingFormatName.Enumeration;
		}
	}
}
