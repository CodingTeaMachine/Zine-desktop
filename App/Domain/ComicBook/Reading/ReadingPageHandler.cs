using System.Data;
using System.Timers;
using ElectronNET.API;
using Microsoft.JSInterop;
using Zine.App.Domain.ComicBookPageInformation;
using Zine.App.Enums;
using Zine.App.Helpers.Canvas;
using Timer = System.Timers.Timer;

namespace Zine.App.Domain.ComicBook.Reading;

public class ReadingPageHandler : IAsyncDisposable
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

	private Timer _readingTimer = new(1000);

	private Action? _userActive;
	private Action? _userIdle;

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
			_canvasHandler = new CanvasHandler(handlerParams.JsRuntime, handlerParams.CanvasId),
			_userActive = handlerParams.UserActive,
			_userIdle = handlerParams.UserIdle,
		};

		handler.LoadComic(handlerParams.ComicBookId);
		handlerParams.ComicBookInformationService.UpdateLastReadTimeToCurrentTime(handler.ComicBook.Information.Id);
		
		handler.InitTimer();

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
			
			//Restart the timer, so we don't get spillover time
			ResetTimer();
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

			//If the dictionary already contains the key, this removes it
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
			.Select(p =>
			{
				
				var page = new Page
				{
					PageInformation = p,
					PageNumberStart = pageNumber,
					Image = GetFilename(p),
					PageNumberEnd = p.PageType == PageType.Double ? ++pageNumber : pageNumber,
				};

				pageNumber++;
				
				return page;
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
		return Path.Join(DataPath.ComicBookReadingDirectoryFromAssetRoot, Uri.EscapeDataString(pageName));
	}
	private void InitTimer()
	{
		_readingTimer = new Timer(1000);
		_readingTimer.Elapsed += IncreaseSecondsRead;
		_readingTimer.AutoReset = true;
		StartTimer();

		SubscribeToWindowEvents();
	}

	private void IncreaseSecondsRead(object? source, ElapsedEventArgs e)
	{
		CurrentPage.PageInformation.TimeSpentReadingPage += TimeSpan.FromSeconds(1);
		_uiUpdateHandler?.Invoke();
	}
	
	public void StartTimer()
	{
		try
		{
			
			_readingTimer?.Start();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			throw;
		}
	}

	public void StopTimer()
	{
		_readingTimer?.Stop();
	}


	private void ResetTimer()
	{
		StopTimer();
		StartTimer();
	}

	private void SubscribeToWindowEvents()
	{
		var mainWindow = Electron.WindowManager.BrowserWindows.First();
		mainWindow.OnBlur += OnBlur;
		mainWindow.OnFocus += OnFocus;
	}

	private void UnsubscribeFromWindowEvents()
	{
		var mainWindow = Electron.WindowManager.BrowserWindows.First();
		mainWindow.OnBlur -= OnBlur;
		mainWindow.OnFocus -= OnFocus;
	}

	private void OnFocus()
	{
		_userActive?.Invoke();
		StartTimer();
	}

	private void OnBlur()
	{
		_userIdle?.Invoke();
		StopTimer();
	}

	public async ValueTask DisposeAsync()
	{
		if (_canvasHandler != null)
			await _canvasHandler.DisposeAsync();
		
		_readingTimer?.Dispose();
		UnsubscribeFromWindowEvents();

	}
}
