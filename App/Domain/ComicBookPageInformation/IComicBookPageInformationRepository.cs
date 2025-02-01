namespace Zine.App.Domain.ComicBookPageInformation;

public interface IComicBookPageInformationRepository
{
	public void CreateMany(IEnumerable<ComicBookPageInformation> comicBookPageInformations);
}
