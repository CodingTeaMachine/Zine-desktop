using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Database;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBookPageInformation;

public class ComicBookPageInformationRepository(IDbContextFactory<ZineDbContext> contextFactory,ILoggerService logger) : IComicBookPageInformationRepository
{
	public IEnumerable<ComicBookPageInformation> CreateMany(IEnumerable<ComicBookPageInformation> comicBookPageInformationList, ZineDbContext? context = null)
	{
		context ??= contextFactory.CreateDbContext();

		var bookPageInformationList = comicBookPageInformationList as ComicBookPageInformation[] ?? comicBookPageInformationList.ToArray();

		context.ComicBookPageInformation.AddRange(bookPageInformationList);
		try
		{
			context.SaveChanges();
			logger.Information($"Created {bookPageInformationList.Length} comic book page informations for comic: ${bookPageInformationList.First().ComicBookId}");

			return bookPageInformationList;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error creating pages", Severity.Error, e.Message);
		}
	}
}
