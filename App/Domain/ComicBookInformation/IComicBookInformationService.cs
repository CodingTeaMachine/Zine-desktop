using Zine.App.Database;

namespace Zine.App.Domain.ComicBookInformation;

public interface IComicBookInformationService
{
	//Create
	public ComicBookInformation Create(string comicBookPathOnDisk, int comicBookId, ComicBookPageInformation.ComicBookPageInformation coverPageInformation);

	//Update
	public void UpdateLastReadTimeToCurrentTime(int comicBookInformationId);
}
