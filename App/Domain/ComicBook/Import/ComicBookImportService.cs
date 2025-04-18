using MudBlazor;
using Zine.App.Domain.ComicBook.Import.Events;
using Zine.App.Enums;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook.Import;

public class ComicBookImportService(
	ImportStrategyFactory importStrategyFactory,
	ImportEventService eventService) : IComicBookImportService
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="action"></param>
	/// <param name="groupId"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="FormatException"></exception>
	/// <exception cref="DirectoryNotFoundException"></exception>
	/// TODO: Handle exceptions in a nicer way
	public void ImportFromDisk(ImportAction action, int groupId)
	{

		switch (action.Type)
		{
			case ImportType.File when !File.Exists(action.FilePath):
				throw new DirectoryNotFoundException("File doesn't exist: " + action.FilePath);
			case ImportType.Directory when !Directory.Exists(action.FilePath):
				throw new DirectoryNotFoundException("Directory doesn't exist: " + action.FilePath);
		}

		var strategy =
			importStrategyFactory.GetStrategy(action.Type, action.IsRecursiveImport, action.KeepFoldersAsGroups);

		if(strategy == null)
			throw new HandledAppException("Cannot determine import strategy", Severity.Error);

		var context = new ImportContext(strategy);

		var numberOfImports = context.GetNumberOfImports(action.FilePath);
		eventService.NotifyTotalCountSet(numberOfImports);

		context.Execute(action.FilePath, groupId);
	}
}
