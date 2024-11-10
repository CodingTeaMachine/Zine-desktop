namespace Zine.App.Domain.ComicBookInformation;

public interface IComicBookInformationService
{
	public ComicBookInformation Create(string comicBookPathOnDisk, int comicBookId);
}
