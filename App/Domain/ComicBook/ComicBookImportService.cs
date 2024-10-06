using System.Data;
using Zine.App.Domain.ComicBook.CompressionFormatHandler;
using Zine.App.Enums;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook;

public class ComicBookImportService(IComicBookRepository comicBookRepository, ILoggerService logger) : IComicBookImportService
{

	public bool ImportFromDisk(ImportType importType ,string pathOnDisk, int groupId, bool recursiveImport = false)
	{
		logger.Information($"Importing {(importType == ImportType.Directory ? "directory" : "file")} from: {pathOnDisk}");

		return importType switch
		{
			ImportType.File => ImportFileFromDisk(pathOnDisk, groupId),
			ImportType.Directory => ImportDirectoryFromDisk(pathOnDisk, groupId, recursiveImport),
			_ => throw new ArgumentOutOfRangeException(nameof(importType), importType, null)
		};
	}


	private bool ImportFileFromDisk(string pathOnDisk, int groupId)
	{
		try
		{
			var format = ComicBookCompressionFormatFactory.GetFromFilePathOrName(pathOnDisk);

			ComicBookInformationFactory comicBookInformationFactory = new(logger);
			var coverImageName = comicBookInformationFactory.GetCoverImage(pathOnDisk, format);

			var cbInfo = new ComicBookInformation.ComicBookInformation
			{
				CoverImage = coverImageName,
				PageNamingFormat = (int)format,
				NumberOfPages = comicBookInformationFactory.GetNumberOfPages(pathOnDisk)
			};

			comicBookRepository.Create(
				Path.GetFileNameWithoutExtension(pathOnDisk),
				pathOnDisk,
				cbInfo,
				groupId
			);

			return true;
		}
		catch (DataException)
		{
			return false;
		}
	}

	private bool ImportDirectoryFromDisk(string pathOnDisk, int groupId, bool recursiveImport)
	{
		ComicBookInformationFactory comicBookInformationFactory = new(logger);

		SearchOption searchDepth = recursiveImport
			? SearchOption.AllDirectories
			: SearchOption.TopDirectoryOnly;

		List<ComicBook> comicBookFiles = Directory.EnumerateFiles(pathOnDisk, "*.cb?", searchDepth)
			.Where(filePath => ComicBookCompressionFormatFactory.ComicFileExtensions.Contains(Path.GetExtension(filePath)))
			.Select(filePath =>
			{
				var format = ComicBookCompressionFormatFactory.GetFromFilePathOrName(filePath);
				var coverImageName = comicBookInformationFactory.GetCoverImage(filePath, format);

				var cbInfo = new ComicBookInformation.ComicBookInformation
				{
					PageNamingFormat = (int)format,
					CoverImage = coverImageName,
					NumberOfPages = comicBookInformationFactory.GetNumberOfPages(pathOnDisk)
				};

				var cb = new ComicBook
				{
					Name = Path.GetFileNameWithoutExtension(filePath),
					FileUri = filePath,
					GroupId = groupId,
					Information = cbInfo
				};

				return cb;
			})
			.ToList();

		comicBookRepository.CreateMany(comicBookFiles);

		return true;
	}
}
