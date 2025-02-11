namespace Zine.App.Domain.ComicBookPageInformation;

public class ComicBookPageInformationService(IComicBookPageInformationRepository repository) : IComicBookPageInformationService
{
	public void CreateMany(string comicBookPathOnDisk, int comicBookId)
	{
		var pagesToCreate = PageInfoListFactory.GetPageInfoList(comicBookPathOnDisk, comicBookId);
		repository.CreateMany(pagesToCreate);
	}

}
