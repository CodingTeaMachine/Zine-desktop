using Zine.App.Domain.ComicBook.Import.Events;

namespace Zine.App.Domain.ComicBook.Import.Strategies;

public class SingleFileImportStrategy(ImportUnitOfWork unitOfWork, ImportEventService eventService) : AImportStrategy(unitOfWork, eventService)
{

	private readonly ImportUnitOfWork _unitOfWork = unitOfWork;

	public override int GetNumberOfImports(string directoryPath)
	{
		return 1;
	}


	public override void Import(string comicBookPathOnDisk, int groupId)
	{
		_unitOfWork.Logger.Information($"SingleFileImportStrategy.Import: Importing comic book {comicBookPathOnDisk}");
		ImportComicBook(comicBookPathOnDisk, groupId);
	}
}
