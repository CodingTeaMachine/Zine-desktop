namespace Zine.App.Domain.ComicBook.Reading;

public struct Page
{
	public ComicBookPageInformation.ComicBookPageInformation PageInformation { get; set; }
	public string Image { get; set; }
	public int PageNumberStart { get; set; }
	public int PageNumberEnd { get; set; }
}
