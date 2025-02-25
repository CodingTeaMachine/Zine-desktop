namespace Zine.App.Domain.ComicBook.Import.Events;

public class ImportEventService
{
	public event EventHandler<int>? OnTotalCountSet;
	public event Action? OnImport;

	public void NotifyTotalCountSet(int totalCount)
	{
		OnTotalCountSet?.Invoke(this, totalCount);
	}

	public void NotifyImport()
	{
		OnImport?.Invoke();
	}
}
