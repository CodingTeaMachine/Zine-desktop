using Microsoft.EntityFrameworkCore;
using Zine.App.Database;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBookInformation;

public class ComicBookInformationRepository(IDbContextFactory<ZineDbContext> contextFactory,ILoggerService logger) : Repository(contextFactory), IComicBookInformationRepository
{
	public ComicBookInformation Create(int comicBookId, string coverImageName, int pageNamingFormat, int numberOfPages)
	{
		var cbInfo = new ComicBookInformation
		{
			ComicBookId = comicBookId,
			CoverImage = coverImageName,
			PageNamingFormat = pageNamingFormat,
			NumberOfPages = numberOfPages
		};

		var dbContext = GetDbContext();
		var savedComicBookInformation = dbContext.ComicBookInformation.Add(cbInfo);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"Created comic book information: \"{savedComicBookInformation.Entity.Id}\"");
			return savedComicBookInformation.Entity;
		}
		catch (DbUpdateException e)
		{
			Console.WriteLine(e);
			throw;
		}
		finally
		{
			DisposeDbContext();
		}

	}
}
