namespace Zine.App.Domain.ComicBook.Import.Strategies;

public class SingleFileImportStrategy(ImportUnitOfWork unitOfWork) : AImportStrategy(unitOfWork)
{

	private readonly ImportUnitOfWork _unitOfWork = unitOfWork;

	public override void Import(string comicBookPathOnDisk, int groupId)
	{
		_unitOfWork.Logger.Information($"SingleFileImportStrategy.Import: Importing comic book {comicBookPathOnDisk}");
		ImportProcess(comicBookPathOnDisk, groupId);
	}
}
