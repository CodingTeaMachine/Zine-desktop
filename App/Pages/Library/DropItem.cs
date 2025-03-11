using Zine.App.Domain.ComicBook;

namespace Zine.App.Pages.Library;

public class DropItem
{
	public ComicBook ComicBook { get; init; } = null!;
	public string Identifier { get; init; } = null!;
}
