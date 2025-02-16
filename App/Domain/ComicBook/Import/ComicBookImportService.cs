using SharpCompress;
using Zine.App.Database;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;
using Zine.App.Domain.ComicBookPageInformation;
using Zine.App.Enums;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook.Import;

public class ComicBookImportService(
	IComicBookService comicBookService,
	IComicBookInformationService comicBookInformationService,
	IComicBookPageInformationService comicBookPageInformationService,
	ZineDbContext context,
	ILoggerService logger) : IComicBookImportService
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
	/// TODO: Handle exceptions in a nicer way
	public List<string>? ImportFromDisk(ImportType importType ,string pathOnDisk, int groupId, bool recursiveImport = false)
	{
		logger.Information($"Importing {(importType == ImportType.Directory ? "directory" : "file")} from: {pathOnDisk}");

		switch (importType)
		{
			case ImportType.File:
				try
				{
					ImportFileFromDisk(pathOnDisk, groupId);
					return null; // No error
				}
				catch (FormatException)
				{
					return [Path.GetFileNameWithoutExtension(pathOnDisk)];
				}
			case ImportType.Directory:
				var errorList = ImportDirectoryFromDisk(pathOnDisk, groupId, recursiveImport);
				return errorList.Count > 0
					? errorList
					: null; // No error
			default:
				throw new ArgumentOutOfRangeException(nameof(importType), importType, "Unsupported import type");
		}
	}


	/// <summary>
	///
	/// </summary>
	/// <param name="comicBookPathOnDisk"></param>
	/// <param name="groupId"></param>
	/// <exceptions cref="FormatException"></exceptions>
	private void ImportFileFromDisk(string comicBookPathOnDisk, int groupId)
	{
		if (!CompressionFormatFactory.IsSupportedFormat(comicBookPathOnDisk))
		{
			throw new FormatException("Unsupported compression format");
		}

		using var transaction = context.Database.BeginTransaction();

		try
		{
			var createdComicBook = comicBookService.Create(
				Path.GetFileNameWithoutExtension(comicBookPathOnDisk),
				comicBookPathOnDisk,
				groupId
			);

			var createdPageInfo =  comicBookPageInformationService.CreateMany(comicBookPathOnDisk, createdComicBook.Id);

			var coverImage = createdPageInfo.First(info => info.PageType == PageType.Cover);
			comicBookInformationService.Create(comicBookPathOnDisk, createdComicBook.Id, coverImage);

			transaction.Commit();
		}
		catch (Exception e)
		{
			logger.Error($"Failed to import file {comicBookPathOnDisk}. Rolling back transaction. {e.Message}");
			transaction.Rollback();
			throw;
		}
	}

	private List<string> ImportDirectoryFromDisk(string pathOnDisk, int groupId, bool recursiveImport)
	{
		var unsupportedComicBookList = new List<string>();

		var searchDepth = recursiveImport
			? SearchOption.AllDirectories
			: SearchOption.TopDirectoryOnly;

		Directory.EnumerateFiles(pathOnDisk, "*.cb?", searchDepth)
			.ForEach(filePath =>
			{
				try
				{
					ImportFileFromDisk(filePath, groupId);
				}
				catch (FormatException)
				{
					unsupportedComicBookList.Add(Path.GetFileNameWithoutExtension(filePath));
				}
			});

		return unsupportedComicBookList;
	}
}
