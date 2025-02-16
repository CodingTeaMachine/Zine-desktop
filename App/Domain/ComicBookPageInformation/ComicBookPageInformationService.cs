using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Database;
using Zine.App.Exceptions;

namespace Zine.App.Domain.ComicBookPageInformation;

public class ComicBookPageInformationService(
	ZineDbContext dbContext,
	GenericRepository<ComicBookPageInformation> repository
	) : IComicBookPageInformationService
{
	public IEnumerable<ComicBookPageInformation> CreateMany(string comicBookPathOnDisk, int comicBookId)
	{
		var pagesToCreate = PageInfoListFactory.GetPageInfoList(comicBookPathOnDisk, comicBookId);

		try
		{
			repository.InsertMany(pagesToCreate);
			dbContext.SaveChanges();
			return pagesToCreate;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Failed to create comic book page informations", Severity.Error, e);
		}

	}

}
