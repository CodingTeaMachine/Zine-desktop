namespace Zine.App.Domain.ComicBookInformation;

public interface IComicBookInformationRepository
{
	public ComicBookInformation Create(int comicBookId, int pageNamingFormat);
}
