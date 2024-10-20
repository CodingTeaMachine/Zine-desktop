using Zine.App.Domain.ComicBookInformation.CompressionFormatHandler;
using Zine.App.Enums;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook;

public class ComicBookImportService(IComicBookRepository comicBookRepository, ILoggerService logger) : IComicBookImportService
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

		switch(importType)
		{
			case ImportType.File:
				ImportFileFromDisk(pathOnDisk, groupId);
				break;
			case ImportType.Directory:
				ImportDirectoryFromDisk(pathOnDisk, groupId, recursiveImport);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(importType), importType, "Import type not supported");
		};
	}


	/// <summary>
	///
	/// </summary>
	/// <param name="pathOnDisk"></param>
	/// <param name="groupId"></param>
	private void ImportFileFromDisk(string pathOnDisk, int groupId)
	{

		var format = CompressionFormatFactory.GetFromFile(pathOnDisk);

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

	}

	private void ImportDirectoryFromDisk(string pathOnDisk, int groupId, bool recursiveImport)
	{
		ComicBookInformationFactory comicBookInformationFactory = new(logger);

		SearchOption searchDepth = recursiveImport
			? SearchOption.AllDirectories
			: SearchOption.TopDirectoryOnly;

		List<ComicBook> comicBookFiles = Directory.EnumerateFiles(pathOnDisk, "*.cb?", searchDepth)
			.Where(filePath => CompressionFormatFactory.ComicFileExtensions.Contains(Path.GetExtension(filePath)))
			.Select(filePath =>
			{
				var format = CompressionFormatFactory.GetFromFile(filePath);
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
