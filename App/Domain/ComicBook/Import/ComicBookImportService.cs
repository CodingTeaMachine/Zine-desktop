using MudBlazor;
using Zine.App.Enums;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook.Import;

public class ComicBookImportService(
	ImportStrategyFactory importStrategyFactory,
	ILoggerService logger) : IComicBookImportService
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="action"></param>
	/// <param name="groupId"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="FormatException"></exception>
	/// TODO: Handle exceptions in a nicer way
	public void ImportFromDisk(ImportAction action, int groupId)
	{
		logger.Information($"Importing {(action.Type == ImportType.Directory ? "directory" : "file")} from: {action.FilePath}");

		var strategy =
			importStrategyFactory.GetStrategy(action.Type, action.IsRecursiveImport, action.KeepFoldersAsGroups);

		if(strategy == null)
			throw new HandledAppException("Cannot determine import strategy", Severity.Error);

		new ImportContext(strategy).Execute(action.FilePath, groupId);
	}
}
