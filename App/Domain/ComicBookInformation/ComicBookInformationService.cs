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
	GenericRepository<ComicBookInformation> repository,
	GenericRepository<StatusTag.StatusTag> statusTagRepository) : IComicBookInformationService
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

	public void SetStatusTagToFinished(ComicBookInformation comicBookInformation)
	{
		var statusTag = statusTagRepository.First(st => st.Name == "Finished");

		comicBookInformation.StatusTag = statusTag;

		try
		{
			dbContext.Update(comicBookInformation);
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error updating comic book status to finished", Severity.Error, e);
		}

	}
}
