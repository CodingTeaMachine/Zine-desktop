using System.Data;
using SharpCompress;
using Zine.App.Domain.ComicBook.Import.Events;

namespace Zine.App.Domain.ComicBook.Import.Strategies;

public class TopLevelImportStrategy(ImportUnitOfWork unitOfWork, ImportEventService eventService) : AImportStrategy(unitOfWork, eventService)
{

	private readonly ImportUnitOfWork _unitOfWork = unitOfWork;

	public override int GetNumberOfImports(string directoryPath)
	{
		_unitOfWork.Logger.Information($"TopLevelImportStrategy.GetNumberOfComicBooks: Getting number of comic books to import {directoryPath}");

		return GetComicBookPaths(directoryPath).Count();
	}

	public override void Import(string comicBookPathOnDisk, int groupId)
	{
		_unitOfWork.Logger.Information($"TopLevelImportStrategy.Import: Importing comic book {comicBookPathOnDisk}");
		var unsupportedComicBookList = new List<string>();

		GetComicBookPaths(comicBookPathOnDisk)
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

	private IEnumerable<string> GetComicBookPaths(string directoryPath)
	{
		return Directory.EnumerateFiles(directoryPath, "*.cb?", SearchOption.TopDirectoryOnly);
	}
}
