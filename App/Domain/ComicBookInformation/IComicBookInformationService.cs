using Zine.App.Database;
using Zine.App.Exceptions;

namespace Zine.App.Domain.ComicBookInformation;

public interface IComicBookInformationService
{
	//Create
	public ComicBookInformation Create(string comicBookPathOnDisk, int comicBookId, ComicBookPageInformation.ComicBookPageInformation coverPageInformation);

	//Update
	public void UpdateLastReadTimeToCurrentTime(int comicBookInformationId);

	/// <summary>
	///
	/// </summary>
	/// <param name="comicBookInformation"></param>
	/// <exception cref="HandledAppException">Thrown when could not update comic book information status tag</exception>
	public void SetStatusTagToFinished(ComicBookInformation comicBookInformation);
}
