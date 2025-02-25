using Zine.App.Domain.ComicBook.Import.Strategies;

namespace Zine.App.Domain.ComicBook.Import;

public class ImportContext(AImportStrategy strategy)
{
	public void Execute(string pathOnDisc, int groupId)
	{
		strategy.Import(pathOnDisc, groupId);
	}

	public int GetNumberOfImports(string pathOnDisc)
	{
		return strategy.GetNumberOfImports(pathOnDisc);
	}
}
