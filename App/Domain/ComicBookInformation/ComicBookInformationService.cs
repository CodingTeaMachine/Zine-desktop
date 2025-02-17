using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Database;
using Zine.App.Exceptions;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBookInformation;

public class ComicBookInformationService(
	ZineDbContext dbContext,
	GenericRepository<ComicBookInformation> repository,
	ILoggerService logger) : IComicBookInformationService
{
	public ComicBookInformation Create(string comicBookPathOnDisk, int comicBookId, ComicBookPageInformation.ComicBookPageInformation comicBookPageInformation)
	{
		ComicBookImageHandler comicBookImageHandler = new(logger);
		var savedCoverImageName = comicBookImageHandler.SaveThumbnailToDisc(comicBookPathOnDisk, comicBookPageInformation.PageFileName , comicBookId.ToString());

		var infoToSave = new ComicBookInformation
		{
			ComicBookId = comicBookId,
			SavedCoverImageFileName = savedCoverImageName,
		};

		try
		{
			repository.Insert(infoToSave);
			dbContext.SaveChanges();
			return infoToSave;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error creating comic book information", Severity.Error, e);
		}
	}
}
