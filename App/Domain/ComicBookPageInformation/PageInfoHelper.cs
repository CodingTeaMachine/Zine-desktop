using SharpCompress.Archives;

namespace Zine.App.Domain.ComicBookPageInformation;

public class PageInfoHelper(IEnumerable<ComicBookPageInformation> pages)
{
	public ComicBookPageInformation GetCover()
	{
		return GetByPageType(PageType.Cover)!;
	}

	public IArchiveEntry GetFileFromArchiveByPage(IArchive archive, string pageName)
	{
		//The key may contain some path parts that we don't store in the DB
		return archive.Entries.First(entry => !entry.IsDirectory && Path.GetFileName(entry.Key)!.Equals(pageName));
	}

	public ComicBookPageInformation? GetCoverInside()
	{
		return GetByPageType(PageType.CoverInside);
	}

	public ComicBookPageInformation? GetBackCover()
	{
		return GetByPageType(PageType.BackCover);
	}

	public ComicBookPageInformation? GetBackCoverInside()
	{
		return GetByPageType(PageType.BackCoverInside);
	}

	public IEnumerable<ComicBookPageInformation> GetPages()
	{
		return pages.Where(page => page.PageType is PageType.Single or PageType.Double);
	}

	private ComicBookPageInformation? GetByPageType(PageType pageType)
	{
		return pages.FirstOrDefault(page => page.PageType == pageType);
	}
}
