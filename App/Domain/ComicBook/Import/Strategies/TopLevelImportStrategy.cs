using System.Data;
using SharpCompress;

namespace Zine.App.Domain.ComicBook.Import.Strategies;

public class TopLevelImportStrategy(ImportUnitOfWork unitOfWork) : AImportStrategy(unitOfWork)
{

	private readonly ImportUnitOfWork _unitOfWork = unitOfWork;

	public override void Import(string comicBookPathOnDisk, int groupId)
	{
		_unitOfWork.Logger.Information($"TopLevelImportStrategy.Import: Importing comic book {comicBookPathOnDisk}");
		var unsupportedComicBookList = new List<string>();

		Directory.EnumerateFiles(comicBookPathOnDisk, "*.cb?", SearchOption.TopDirectoryOnly)
			.ForEach(filePath =>
			{
				try
				{
					ImportComicBook(filePath, groupId);
				}
				catch (FormatException)
				{
					unsupportedComicBookList.Add(Path.GetFileNameWithoutExtension(filePath));
				}
			});

		if (unsupportedComicBookList.Count != 0)
			throw new DataException("Error importing the following comic books: " + string.Join(", ", unsupportedComicBookList));
	}
}
