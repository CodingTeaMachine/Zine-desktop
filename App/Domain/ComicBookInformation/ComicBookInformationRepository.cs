using Microsoft.EntityFrameworkCore;
using Zine.App.Database;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBookInformation;

public class ComicBookInformationRepository(IDbContextFactory<ZineDbContext> contextFactory,ILoggerService logger) : IComicBookInformationRepository
{
	public ComicBookInformation Create(int comicBookId, int pageNamingFormat)
	{
		var cbInfo = new ComicBookInformation
		{
			ComicBookId = comicBookId,
			PageNamingFormat = pageNamingFormat,
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
