using Zine.App.Domain.ComicBook.Import.Strategies;
using Zine.App.Enums;

namespace Zine.App.Domain.ComicBook.Import;

public class ImportStrategyFactory(IServiceProvider serviceProvider)
{

	public AImportStrategy? GetStrategy(ImportType importType, bool isRecursiveImport, bool keepDirectoryStructure)
	{
		if(importType == ImportType.File)
			return serviceProvider.GetService<SingleFileImportStrategy>();

		if (keepDirectoryStructure)
			return serviceProvider.GetService<KeepStructureImportStrategy>();

		if(isRecursiveImport)
			return serviceProvider.GetService<IncludeSubdirectoriesImportStrategy>();

		return serviceProvider.GetService<TopLevelImportStrategy>();
	}
}
