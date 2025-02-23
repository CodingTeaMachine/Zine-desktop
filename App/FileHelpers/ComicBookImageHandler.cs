using System.Data;
using SharpCompress.Archives;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Enums;
using Zine.App.Helpers;
using Zine.App.Logger;

namespace Zine.App.FileHelpers;

public class ComicBookImageHandler(ILoggerService? logger = null)
{
	private const int ThumbnailImageHeight = 369;
	private const int ThumbnailImageWidth = 240;
	
	public void SaveThumbnailToDisc(string comicBookFilePath, string coverImageFileName, string outputFileName, string outputDirectory = DataPath.ComicBookCoverDirectory)
	{
		if (!Directory.Exists(outputDirectory))
			Directory.CreateDirectory(outputDirectory);

		var fullOutputFilePath = Path.Join(outputDirectory, outputFileName);
		WriteImageToDisc(comicBookFilePath, coverImageFileName, fullOutputFilePath);

		logger?.Information($"Importing cover image for: {Path.GetFileNameWithoutExtension(coverImageFileName)}. Image name: {outputFileName}");
	}

	private void WriteImageToDisc(string filePath, string coverImageFileName, string filePathToSave)
	{
		var coverImage = CompressedComicBookPageInformationExtractor.GetCoverImage(filePath, coverImageFileName);

		try
		{
			var resizedImageData = Image.GetResizedImage(coverImage, ThumbnailImageHeight, ThumbnailImageWidth);
			File.WriteAllBytes(filePathToSave, resizedImageData);
			logger?.Information($"Saving resized image to disc ({ThumbnailImageHeight}x{ThumbnailImageWidth}, {filePathToSave})");
		}
		catch (Exception ex) when (ex is NotSupportedException or DataException)
		{
			//Simply save the original image
			var originalDimensions = Image.GetDimensions(coverImage);
			logger?.Warning($"Could not resize image, writing original size to disc ({originalDimensions.Height}x{originalDimensions.Width})");
			coverImage.WriteToFile(filePathToSave);
		}
	}

}
