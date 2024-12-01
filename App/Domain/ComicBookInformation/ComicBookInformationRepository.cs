using Microsoft.EntityFrameworkCore;
using Zine.App.Database;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBookInformation;

public class ComicBookInformationRepository(IDbContextFactory<ZineDbContext> contextFactory,ILoggerService logger) : IComicBookInformationRepository
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

		using var context = contextFactory.CreateDbContext();
		var savedComicBookInformation = context.ComicBookInformation.Add(cbInfo);

		try
		{
			context.SaveChanges();
			logger.Information($"Created comic book information: \"{savedComicBookInformation.Entity.Id}\"");
			return savedComicBookInformation.Entity;
		}
		catch (DbUpdateException e)
		{
			//TODO: Error handling
			Console.WriteLine(e);
			throw;
		}
	}
}
