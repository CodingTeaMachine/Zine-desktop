using System.Data;
using SharpCompress;
using Zine.App.Domain.ComicBook.Import.Events;

namespace Zine.App.Domain.ComicBook.Import.Strategies;

public class IncludeSubdirectoriesImportStrategy(ImportUnitOfWork unitOfWork, ImportEventService eventService) : AImportStrategy(unitOfWork, eventService)
{
	private readonly ImportUnitOfWork _unitOfWork = unitOfWork;

	public override int GetNumberOfImports(string directoryPath)
	{
		_unitOfWork.Logger.Information($"RecursiveImportStrategy.GetNumberOfComicBooks: Getting number of comic books to import {directoryPath}");

		return GetComicBookPaths(directoryPath).Count();
	}

	public override void Import(string comicBookPathOnDisk, int groupId)
	{
		_unitOfWork.Logger.Information($"RecursiveImportStrategy.Import: Importing comic book {comicBookPathOnDisk}");
		var importErrorList = new List<string>();

		GetComicBookPaths(comicBookPathOnDisk)
			.ForEach(filePath =>
			{
				try
				{
					ImportComicBook(filePath, groupId);
				}
				catch (Exception)
				{
					importErrorList.Add(Path.GetFileNameWithoutExtension(filePath));
				}
			});

		if (importErrorList.Count != 0)
			throw new DataException("Error importing the following comic books: " + string.Join(", ", importErrorList));
	}

	private List<string> GetComicBookPaths(string directoryPath)
	{
		return Directory.EnumerateFiles(directoryPath, "*.cb?", SearchOption.AllDirectories).ToList();
	}
}
