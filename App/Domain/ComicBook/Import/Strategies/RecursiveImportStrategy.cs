using System.Data;
using SharpCompress;

namespace Zine.App.Domain.ComicBook.Import.Strategies;

public class RecursiveImportStrategy(ImportUnitOfWork unitOfWork) : AImportStrategy(unitOfWork)
{
	private readonly ImportUnitOfWork _unitOfWork = unitOfWork;

	public override void Import(string comicBookPathOnDisk, int groupId)
	{
		_unitOfWork.Logger.Information($"RecursiveImportStrategy.Import: Importing comic book {comicBookPathOnDisk}");
		var importErrorList = new List<string>();

		Directory.EnumerateFiles(comicBookPathOnDisk, "*.cb?", SearchOption.AllDirectories)
			.ForEach(filePath =>
			{
				try
				{
					ImportProcess(filePath, groupId);
				}
				catch (Exception)
				{
					importErrorList.Add(Path.GetFileNameWithoutExtension(filePath));
				}
			});

		if (importErrorList.Count != 0)
			throw new DataException("Error importing the following comic books: " + string.Join(", ", importErrorList));
	}
}
