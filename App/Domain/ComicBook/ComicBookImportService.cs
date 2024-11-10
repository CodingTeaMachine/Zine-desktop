using SharpCompress;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;
using Zine.App.Enums;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook;

public class ComicBookImportService(IComicBookRepository comicBookRepository, IComicBookInformationService comicBookInformationService , ILoggerService logger) : IComicBookImportService
{
	/// <summary>
	///
	/// </summary>
	/// <param name="importType"></param>
	/// <param name="pathOnDisk"></param>
	/// <param name="groupId"></param>
	/// <param name="recursiveImport"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="FormatException"></exception>
	public void ImportFromDisk(ImportType importType ,string pathOnDisk, int groupId, bool recursiveImport = false)
	{

		ValidateInputFormat(pathOnDisk);

		logger.Information($"Importing {(importType == ImportType.Directory ? "directory" : "file")} from: {pathOnDisk}");

		Action importAction = importType switch
		{
			ImportType.File => () => ImportFileFromDisk(pathOnDisk, groupId),
			ImportType.Directory => () => ImportDirectoryFromDisk(pathOnDisk, groupId, recursiveImport),
			_ => throw new ArgumentOutOfRangeException(nameof(importType), importType, "Import type not supported"),
		};
		
		importAction();
	}


	/// <summary>
	///
	/// </summary>
	/// <param name="comicBookPathOnDisk"></param>
	/// <param name="groupId"></param>
	private void ImportFileFromDisk(string comicBookPathOnDisk, int groupId)
	{
		var createdComicBook = comicBookRepository.Create(
			Path.GetFileNameWithoutExtension(comicBookPathOnDisk),
			comicBookPathOnDisk,
			groupId
		);

		comicBookInformationService.Create(comicBookPathOnDisk, createdComicBook.Id);
	}

	private void ImportDirectoryFromDisk(string pathOnDisk, int groupId, bool recursiveImport)
	{
		var searchDepth = recursiveImport
			? SearchOption.AllDirectories
			: SearchOption.TopDirectoryOnly;

		Directory.EnumerateFiles(pathOnDisk, "*.cb?", searchDepth)
			.Where(filePath => CompressionFormatFactory.ComicFileExtensions.Contains(Path.GetExtension(filePath)))
			.ForEach(filePath => ImportFileFromDisk(filePath, groupId));
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="pathOnDisk"></param>
	/// <exception cref="FormatException"></exception>
	private void ValidateInputFormat(string pathOnDisk)
	{
		var compressionFormat = CompressionFormatFactory.GetFromFile(pathOnDisk);
		if (compressionFormat == CompressionFormat.Unknown)
		{
			throw new FormatException($"Unsupported compression format ({compressionFormat})");
		}
	}
}
