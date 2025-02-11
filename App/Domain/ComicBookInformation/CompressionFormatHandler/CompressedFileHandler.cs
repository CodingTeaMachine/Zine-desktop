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

	private static readonly string[] ExcludedFirstFileEndings = ["101", "100"];

	public static bool IsCoverImage(IArchiveEntry potentialCoverImage, PageNamingConvention pageNamingConvention)
	{
		if (!Image.DotExtensions.Contains(Path.GetExtension(potentialCoverImage.Key)))
			return false;

		var imageName = Path.GetFileNameWithoutExtension(potentialCoverImage.Key)?.ToLower();

		if (pageNamingConvention == PageNamingConvention.Enumeration)
		{
			return
				!ExcludedFirstFileEndings.Any(excludedFirstFileEnding => imageName!.EndsWith(excludedFirstFileEnding))
				&& PageFormatRegex().IsMatch(imageName!);
		}

		var regex = ComicBookPageNamingFormat.CoverPageFormatToRegexDic[pageNamingConvention];

		return IsMatch(imageName!, regex);
	}

	public static bool IsRearImage(IArchiveEntry potentialCoverImage, PageNamingConvention pageNamingConvention)
	{
		if (!Image.DotExtensions.Contains(Path.GetExtension(potentialCoverImage.Key)))
			return false;

		var imageName = Path.GetFileNameWithoutExtension(potentialCoverImage.Key)?.ToLower();

		if (pageNamingConvention == PageNamingConvention.Enumeration)
		{
			return
				!ExcludedFirstFileEndings.Any(excludedFirstFileEnding => imageName!.EndsWith(excludedFirstFileEnding))
				&& PageFormatRegex().IsMatch(imageName!);
		}

		var regex = ComicBookPageNamingFormat.BackCoverPageFormatToRegexDic[pageNamingConvention];

		return IsMatch(imageName!, regex);
	}

	public static PageNamingConvention GetPageNamingFormat(IArchive comicBookArchive)
	{
		var filenames = comicBookArchive.Entries.Select(comicBookImage => Path.GetFileNameWithoutExtension(comicBookImage.Key)).ToList();

		try
		{
			var regexToPageFormat =
				ComicBookPageNamingFormat
					.CoverPageFormatToRegexDic
					.First(regexToPageFormat =>
						filenames.Any(filename => IsMatch(filename!, regexToPageFormat.Value)));

			return regexToPageFormat.Key;
		}
		catch (InvalidOperationException)
		{
			return PageNamingConvention.Enumeration;
		}
	}

	public string ExtractCoverImage(string outputFileName)
	{
		var infoExtractor = new CompressedComicBookInformationExtractor();
		var coverImage = infoExtractor.GetCoverImage(outputFileName);

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
}
