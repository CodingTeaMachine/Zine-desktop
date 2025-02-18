namespace Zine.App.Domain.ComicBook.Import.Strategies;

public class KeepStructureImportStrategy(ImportUnitOfWork unitOfWork) : AImportStrategy(unitOfWork)
{
	private readonly ImportUnitOfWork _unitOfWork = unitOfWork;

	public override void Import(string comicBookPathOnDisk, int groupId)
	{
		_unitOfWork.Logger.Information($"KeepStructureImportStrategy.Import: Importing comic book {comicBookPathOnDisk}");
		throw new NotImplementedException();
	}
}
