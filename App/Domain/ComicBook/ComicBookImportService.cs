using System.Data;
using Zine.App.Domain.ComicBook.FormatHandler;
using Zine.App.Enums;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook;

public class ComicBookImportService(IComicBookRepository comicBookRepository, ILoggerService logger) : IComicBookImportService
{

	public bool ImportFromDisk(ImportType importType ,string pathOnDisk, int? groupId)
	{
		logger.Information($"Importing {(importType == ImportType.Directory ? "directory" : "file")} from: {pathOnDisk}");

		return importType switch
		{
			ImportType.File => ImportFileFromDisk(pathOnDisk, groupId),
			ImportType.Directory => ImportDirectoryFromDisk(pathOnDisk, groupId),
			_ => throw new ArgumentOutOfRangeException(nameof(importType), importType, null)
		};
	}


	private bool ImportFileFromDisk(string pathOnDisk, int? groupId)
	{
		try
		{
			SymlinkCreator.CreateComicBookLink(pathOnDisk);

			var format = ComicBookFormatFactory.GetFromFilePathOrName(pathOnDisk);

			ComicBookInformationFactory comicBookInformationFactory = new(logger);
			var coverImageName = comicBookInformationFactory.GetCoverImage(pathOnDisk, format);

			var cbInfo = new ComicBookInformation.ComicBookInformation
			{
				CoverImage = coverImageName,
				PageNamingFormat = (int)format
			};

			comicBookRepository.Create(
				Path.GetFileNameWithoutExtension(pathOnDisk),
				Path.GetFileName(pathOnDisk),
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

	private bool ImportDirectoryFromDisk(string pathOnDisk, int? groupId)
	{
		ComicBookInformationFactory comicBookInformationFactory = new(logger);

		List<ComicBook> comicBookFiles = Directory.EnumerateFiles(pathOnDisk, "*.cb?", SearchOption.AllDirectories)
			.Where(filePath => ComicBookFormatFactory.ComicFileExtensions.Contains(Path.GetExtension(filePath)))
			.Select(filePath =>
			{
				SymlinkCreator.CreateComicBookLink(filePath);
				var format = ComicBookFormatFactory.GetFromFilePathOrName(filePath);
				var coverImageName = comicBookInformationFactory.GetCoverImage(filePath, format);

				var cbInfo = new ComicBookInformation.ComicBookInformation
				{
					CoverImage = coverImageName,
					PageNamingFormat = (int)format
				};

				return (filePath, cbInfo);
			})
			.Select(cbData => new ComicBook
			{
				Name = Path.GetFileNameWithoutExtension(cbData.filePath),
				FileName = Path.GetFileName(cbData.filePath),
				GroupId = groupId,
				Information = cbData.cbInfo
			})
			.ToList();

		comicBookRepository.CreateMany(comicBookFiles);

		return true;
	}
}