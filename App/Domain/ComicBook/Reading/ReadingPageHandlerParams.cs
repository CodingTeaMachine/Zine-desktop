using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Domain.ComicBookPageInformation;

namespace Zine.App.Domain.ComicBook.Reading;

public class ReadingPageHandlerParams
{
	public required NavigationManager NavigationManager { get; init; }
	public required IJSRuntime JsRuntime { get; init; }
	public required Action UiUpdateHandler { get; init; }

	public required IComicBookService ComicBookService { get; init; }
	public required IComicBookInformationService ComicBookInformationService { get; init; }
	public required IComicBookPageInformationService ComicBookPageInformationService { get; init; }

	public required string CanvasId { get; init; }
	public required int GroupId { get; init; }
	public required int ComicBookId { get; init; }
}
