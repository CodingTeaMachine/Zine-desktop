using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Database;
using Zine.App.Enums;
using Zine.App.Exceptions;
using Zine.App.FileHelpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBookInformation;

public class ComicBookInformationService(
	ZineDbContext dbContext,
	GenericRepository<ComicBookInformation> repository) : IComicBookInformationService
{
	public ComicBookInformation Create(string comicBookPathOnDisk, int comicBookId, ComicBookPageInformation.ComicBookPageInformation comicBookPageInformation)
	{
		var fileName = comicBookId + Path.GetExtension(comicBookPageInformation.PageFileName).ToLower(); //Sometimes the extension are capitalized

		var infoToSave = new ComicBookInformation
		{
			ComicBookId = comicBookId,
			SavedCoverImageFileName = fileName,
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

	public void UpdateLastReadTimeToCurrentTime(int comicBookInformationId)
	{
		var comicBookInformation = repository.GetById(comicBookInformationId);

		if(comicBookInformation == null)
			throw new HandledAppException("Comic book information doesn't exist", Severity.Error);

		comicBookInformation.LastOpened = DateTime.Now;

		repository.Update(comicBookInformation);
		dbContext.SaveChanges();
	}
}
