using System.Data;
using SharpCompress;
using Zine.App.Domain.ComicBook.Import.Events;

namespace Zine.App.Domain.ComicBook.Import.Strategies;

public class KeepStructureImportStrategy(ImportUnitOfWork unitOfWork, ImportEventService importEventService) : AImportStrategy(unitOfWork, importEventService)
{
	private readonly ImportUnitOfWork _unitOfWork = unitOfWork;
	
	private readonly List<string> _importErrorFiles = [];

	public override int GetNumberOfImports(string directoryPath)
	{
		_unitOfWork.Logger.Information($"KeepStructureImportStrategy.GetNumberOfComicBooks: Getting number of comic books to import {directoryPath}");

		return GetComicBookPaths(directoryPath).Count();

	}


	public override void Import(string comicBookPathOnDisk, int groupId)
	{
		_unitOfWork.Logger.Information($"KeepStructureImportStrategy.Import: Importing comic book {comicBookPathOnDisk}");

		//Creates the top level directory and runs imports inside that
		var dirInfo = new DirectoryInfo(comicBookPathOnDisk);
		var topLevelGroup = _unitOfWork.GroupService.Create(dirInfo.Name, groupId);

		ImportDirectoryContent(comicBookPathOnDisk, topLevelGroup.Id);

		if (_importErrorFiles.Count != 0)
			throw new DataException("Error importing the following comic books: " + string.Join(", ", _importErrorFiles));
	}

	private void ImportDirectoryContent(string directoryPath, int parentGroupId)
	{
		_unitOfWork.Logger.Information($"KeepStructureImportStrategy.ImportDirectoryContent: Importing directory {directoryPath}");
		var directories = Directory.GetDirectories(directoryPath);

		directories.ForEach(directory =>
		{
			var dirInfo = new DirectoryInfo(directory);
			var createdGroup = _unitOfWork.GroupService.Create(dirInfo.Name, parentGroupId);
			ImportDirectoryContent(directory, createdGroup.Id);
		});

		Directory.EnumerateFiles(directoryPath, "*.cb?", SearchOption.TopDirectoryOnly)
			.ForEach(filePath =>
			{
				try
				{
					ImportComicBook(filePath, parentGroupId);
				}
				catch (Exception)
				{
					_importErrorFiles.Add(Path.GetFileNameWithoutExtension(filePath));
				}
			});
	}

	private IEnumerable<string> GetComicBookPaths(string directoryPath)
	{
		return Directory.EnumerateFiles(directoryPath, "*.cb?", SearchOption.AllDirectories);
	}
}
