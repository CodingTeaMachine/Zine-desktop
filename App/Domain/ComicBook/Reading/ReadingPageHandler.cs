namespace Zine.App.Domain.ComicBook.Reading;

public class ReadingPageHandler()
{
	public ComicBook ComicBook { get; set; } = null!;
	public int CurrentPageIndex { get; set; } = 0;

	public void GoToPage(int pageIndex)
	{
		if (pageIndex >= 0 && pageIndex <= ComicBook.Information.NumberOfPages - 1)
		{
			CurrentPageIndex = pageIndex;
		}
	}

	public void GoToLastPage()
	{
		CurrentPageIndex = ComicBook.Information.NumberOfPages - 1;
	}

	public void GoToFirstPage()
	{
		CurrentPageIndex = 0;
	}

	public void NextPage()
	{
		if (CurrentPageIndex < ComicBook.Information.NumberOfPages - 1)
		{
			CurrentPageIndex++;
		}
	}

	public void PrevPage()
	{
		if (CurrentPageIndex > 0)
		{
			CurrentPageIndex--;
		}
	}
}
