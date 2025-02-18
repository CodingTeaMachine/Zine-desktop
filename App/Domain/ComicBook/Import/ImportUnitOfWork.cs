using Zine.App.Database;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Domain.ComicBookPageInformation;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook.Import;

public class ImportUnitOfWork(
	ZineDbContext context,
	IComicBookService comicBookService,
	IComicBookInformationService comicBookInformationService,
	IComicBookPageInformationService comicBookPageInformationService,
	ILoggerService logger)
{
	public ZineDbContext Context { get; } = context;
	public IComicBookService ComicBookService { get; } = comicBookService;
	public IComicBookInformationService ComicBookInformationService { get; } = comicBookInformationService;
	public IComicBookPageInformationService ComicBookPageInformationService { get; } = comicBookPageInformationService;
	public ILoggerService Logger { get; } = logger;
}
