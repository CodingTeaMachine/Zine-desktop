using System.Data;
using Microsoft.JSInterop;
using Zine.App.Domain.ComicBookPageInformation;
using Zine.App.Helpers.Canvas;

namespace Zine.App.Domain.ComicBook.Reading;

public class ReadingPageHandler
{
	public ComicBook ComicBook { get; set; } = null!;

	public int ZoomScale = 100;

	private IComicBookService? _comicBookService;

	private IComicBookPageInformationService? _comicBookPageInformationService;

	private CanvasHandler? _canvasHandler;

	private IJSRuntime? _jsRuntime;

	private Action? _uiUpdateHandler;

	public Page CurrentPage => Pages[_currentPageIndex];

	public int MaxPageNumber => Pages.Last().PageNumberEnd;

	public List<Page> Pages = [];

	private Dictionary<int, int> _originalPageOrder = new();

	// ReSharper disable once RedundantDefaultMemberInitializer
	private int _currentPageIndex = 0;


	/// <summary>
	///
	/// </summary>
	/// <param name="handlerParams"></param>
	/// <exception cref="DataException"></exception>
	/// <returns></returns>
	public static ReadingPageHandler Create(ReadingPageHandlerParams handlerParams)
	{
		var handler = new ReadingPageHandler
		{
			_comicBookService = handlerParams.ComicBookService,
			_comicBookPageInformationService = handlerParams.ComicBookPageInformationService,
			_jsRuntime = handlerParams.JsRuntime,
			_uiUpdateHandler = handlerParams.UiUpdateHandler,
			_canvasHandler = new CanvasHandler(handlerParams.JsRuntime, handlerParams.CanvasId)
		};

		handler.LoadComic(handlerParams.ComicBookId);
		handlerParams.ComicBookInformationService.UpdateLastReadTimeToCurrentTime(handler.ComicBook.Information.Id);

		return handler;
	}

	public int CurrentPageIndex
	{
		get
		{
			// Only update the page read status if navigating to the page for the first time
			if (CurrentPage.PageInformation.IsRead == false)
			{
				_comicBookPageInformationService!.UpdateReadStatus(CurrentPage.PageInformation.Id);
			}

			return _currentPageIndex;
		}
		private set
		{
			if (value == _currentPageIndex)
				return;

			_currentPageIndex = value;

			SetImageOnCanvas(value);
			ScrollImageToViewInSidebar();
			_ = UpdateZoomScale();
		}
	}

	public void GoToPage(int pageIndex)
	{
		if (pageIndex >= 0 && pageIndex <= ComicBook.Pages.Count - 1)
		{
			CurrentPageIndex = pageIndex;
		}
	}

	public void GoToLastPage()
	{
		CurrentPageIndex = ComicBook.Pages.Count - 1;
	}

	public void GoToFirstPage()
	{
		CurrentPageIndex = 0;
	}

	public void NextPage()
	{
		if (CurrentPageIndex < ComicBook.Pages.Count - 1)
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


	public void RefreshCanvasImage()
	{
		SetImageOnCanvas(CurrentPageIndex);
	}

	public async Task RotateRight()
	{
		await _canvasHandler!.RotateRight();
	}

	public async Task RotateLeft()
	{
		await _canvasHandler!.RotateLeft();
	}

	public async Task ZoomIn()
	{
		await _canvasHandler!.ZoomIn();
		await UpdateZoomScale();
	}

	public async Task ZoomOut()
	{
		await _canvasHandler!.ZoomOut();
		await UpdateZoomScale();
	}

	public async Task UpdateZoomScale()
	{
		ZoomScale = await _canvasHandler!.GetZoomScale();
		_uiUpdateHandler!();
	}

	public async Task SetDotnetHelperReference(DotNetObjectReference<Components.Pages.ReadingPage> dotNetObjectReference)
	{
		await _canvasHandler!.SetDotnetHelperReference(dotNetObjectReference);
	}


	public void LoadComic(int comicBookId)
	{
		var loadedComicBook = _comicBookService!.GetForReadingView(comicBookId);

		if (loadedComicBook == null)
			throw new DataException("Comic book not found");

		ComicBook = loadedComicBook;

		_comicBookService!.ExtractImagesOfComicBook(ComicBook.Id);
		Pages = CreatePageInfo(ComicBook.Pages);

		foreach (var page in Pages)
		{
			var key = page.PageInformation.Id;
			var value = page.PageInformation.Index;

			//If the dictionarry already contains the key, this removes it
            _originalPageOrder.Remove(key);

            _originalPageOrder.Add(key, value);
		}
	}

	public void ResetPageOrder()
	{
		Pages = Pages.Select(p =>
		{
			p.PageInformation.Index = _originalPageOrder[p.PageInformation.Id];
			return p;
		})
			.OrderBy(p => p.PageInformation.Index)
			.ToList();
	}

	public static List<Page> CreatePageInfo(IEnumerable<ComicBookPageInformation.ComicBookPageInformation> pages)
	{
		int pageNumber = 1;

		return pages
			.OrderBy(p => p.Index)
			.Select(p => new Page
			{
				PageInformation = p,
				PageNumberStart = pageNumber++,
				Image = GetFilename(p),
				PageNumberEnd = p.PageType == PageType.Double ? pageNumber++ : pageNumber,
			}).ToList();
	}

	private void SetImageOnCanvas(int imageIndex)
	{
		_ = _canvasHandler!.DrawImage(Pages[imageIndex].Image);
	}

	private void ScrollImageToViewInSidebar()
	{
		_jsRuntime!.InvokeVoidAsync("scrollElementIntoView", "image-" + CurrentPageIndex);
	}

	private static string GetFilename(ComicBookPageInformation.ComicBookPageInformation pageInfo)
	{
		var pageName = Path.GetFileName(pageInfo.PageFileName);

		return "/images/Reading/" + Uri.EscapeDataString(pageName);
	}
}
