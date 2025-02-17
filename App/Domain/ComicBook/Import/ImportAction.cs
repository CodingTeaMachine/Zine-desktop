using Zine.App.Enums;

namespace Zine.App.Domain.ComicBook.Import;

public class ImportAction
{
	public ImportType Type;
	public string FilePath = String.Empty;
	public bool IsRecursiveImport = false;
	public bool KeepFoldersAsGroups = false;
}
