namespace Zine.App.Domain.ComicBookPageInformation;

public interface IComicBookPageInformationService
{
	public IEnumerable<ComicBookPageInformation> CreateMany(string comicBookPathOnDisk, int comicBookId);
	public void CheckPageTypes(ComicBook.ComicBook comicBook);
}
