namespace Zine.App.Domain.ComicBook.Reading;

public class CurrentlyReadingInfo()
{
	public ComicBook ComicBook { get; set; } = null!;
	public int CurrentPage { get; set; } = 1;

	public void NextPage()
	{
		if (CurrentPage < 10)
		{
			CurrentPage++;
		}
	}

	public void PrevPage()
	{
		if (CurrentPage > 1)
		{
			CurrentPage--;
		}
	}
}
