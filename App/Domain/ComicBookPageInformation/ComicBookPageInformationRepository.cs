using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Database;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBookPageInformation;

public class ComicBookPageInformationRepository(IDbContextFactory<ZineDbContext> contextFactory,ILoggerService logger) : IComicBookPageInformationRepository
{
	public void CreateMany(IEnumerable<ComicBookPageInformation> comicBookPageInformations)
	{
		using var context = contextFactory.CreateDbContext();
		context.ComicBookPageInformation.AddRange(comicBookPageInformations);

		try
		{
			context.SaveChanges();
			logger.Information($"Created {comicBookPageInformations.Count()} comic book page informations for comic: ${comicBookPageInformations.First().ComicBookId}");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error creating pages", Severity.Error, e.Message);
		}
	}
}
