using System.Data;
using Microsoft.JSInterop;
using Zine.App.Domain.ComicBookPageInformation;
using Zine.App.Helpers.Canvas;

namespace Zine.App.Domain.ComicBook.Reading;

public class ReadingPageHandler
{
	// public ReadingPageHandler(ReadingPageHandlerParams handlerParams)
	// {
	// 	_comicBookService = handlerParams.ComicBookService;
	// 	_comicBookPageInformationService = handlerParams.ComicBookPageInformationService;
	// 	_jsRuntime = handlerParams.JsRuntime;
	// 	_uiUpdateHandler = handlerParams.UiUpdateHandler;
	// 	_canvasHandler = new CanvasHandler(_jsRuntime, handlerParams.CanvasId);
	//
	// 	try
	// 	{
	// 		LoadComic(handlerParams.ComicBookId);
	// 		handlerParams.ComicBookInformationService.UpdateLastReadTimeToCurrentTime(ComicBook.Information.Id);
	// 	}
	// 	catch (DataException e)
	// 	{
	// 		Console.WriteLine(e.Message);
	// 		handlerParams.NavigationManager.NavigateTo(PageManager.GetLibraryGroupLink(handlerParams.GroupId));
	// 	}
	// }


	private ComicBook ComicBook { get; set; } = null!;

	public int ZoomScale = 100;

	private IComicBookService? _comicBookService;

	private IComicBookPageInformationService? _comicBookPageInformationService;

	private CanvasHandler? _canvasHandler;

	private IJSRuntime? _jsRuntime;

	private Action? _uiUpdateHandler;

	public ComicBookPageInformation.ComicBookPageInformation CurrentPage => ComicBook.Pages.ToList()[_currentPageIndex];

	public int MaxPageNumber => ComicBook.Pages.ToList().Last().PageNumberEnd;

	// ReSharper disable once RedundantDefaultMemberInitializer
	private int _currentPageIndex = 0;

	public List<string> Images = [];


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
			if (CurrentPage.IsRead == false)
			{
				_comicBookPageInformationService!.UpdateReadStatus(CurrentPage.Id);
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

	public async void SetDotnetHelperReference(DotNetObjectReference<Components.Pages.Reading> dotNetObjectReference)
	{
		await _canvasHandler!.SetDotnetHelperReference(dotNetObjectReference);
	}


	private void LoadComic(int comicBookId)
	{
		var loadedComicBook = _comicBookService!.GetForReadingView(comicBookId);

		if(loadedComicBook == null)
			throw new DataException("Comic book not found");

		ComicBook = loadedComicBook;

		LoadImages(comicBookId);
	}


	private void LoadImages(int comicBookId)
	{
		_comicBookService!.ExtractImagesOfComicBook(comicBookId);

		var pageInfoHelper = new PageInfoHelper(ComicBook.Pages);

		var coverImage = pageInfoHelper.GetCover();
		Images.Add(coverImage.PageFileName);

		var coverInside = pageInfoHelper.GetCoverInside();
		if(coverInside != null)
			Images.Add(coverInside.PageFileName);

		var pages = pageInfoHelper.GetPages().OrderBy(p => p.PageFileName);
		Images.AddRange(pages.Select(page => page.PageFileName));

		var backCoverInside = pageInfoHelper.GetBackCoverInside();
		if(backCoverInside != null)
			Images.Add(backCoverInside.PageFileName);

		var backCover = pageInfoHelper.GetBackCover();
		if(backCover != null)
			Images.Add(backCover.PageFileName);

		Images = Images.Select(path => "/images/Reading/" + Path.GetFileName(path)).ToList();
	}

	private void SetImageOnCanvas(int imageIndex)
	{
		_ = _canvasHandler!.DrawImage(Images[imageIndex]);
	}

	private void ScrollImageToViewInSidebar()
	{
		_jsRuntime!.InvokeVoidAsync("scrollElementIntoView", "image-" + CurrentPageIndex);
	}

}
