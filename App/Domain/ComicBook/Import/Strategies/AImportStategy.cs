using Zine.App.Domain.ComicBookPageInformation;
using Zine.App.Helpers.Compression;

namespace Zine.App.Domain.ComicBook.Import.Strategies;

public abstract class AImportStrategy(ImportUnitOfWork unitOfWork)
{
	public abstract void Import(string comicBookPathOnDisk, int groupId);

	protected void ImportComicBook(string comicBookPathOnDisk, int groupId)
	{
		if (!CompressionFormatFactory.IsSupportedFormat(comicBookPathOnDisk))
		{
			throw new FormatException("Unsupported compression format");
		}

		unitOfWork.Logger.Information($"AImportStrategy.ImportComicBook: Importing comic book {Path.GetFileNameWithoutExtension(comicBookPathOnDisk)}");
		using var transaction = unitOfWork.Context.Database.BeginTransaction();

		try
		{
			var createdComicBook = unitOfWork.ComicBookService.Create(
				Path.GetFileNameWithoutExtension(comicBookPathOnDisk),
				comicBookPathOnDisk,
				groupId
			);

			var createdPageInfo =  unitOfWork.ComicBookPageInformationService.CreateMany(comicBookPathOnDisk, createdComicBook.Id);

			var coverImage = new PageInfoHelper(createdPageInfo).GetCover();
			unitOfWork.ComicBookInformationService.Create(comicBookPathOnDisk, createdComicBook.Id, coverImage);

			transaction.Commit();
		}
		catch (Exception e)
		{
			unitOfWork.Logger.Error($"Failed to import file {comicBookPathOnDisk}. Rolling back transaction. {e.Message}");
			transaction.Rollback();
			throw;
		}
	}
}
