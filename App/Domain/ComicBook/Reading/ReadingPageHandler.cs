using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Zine.App.Enums;
using Zine.App.Helpers;
using Zine.App.Helpers.Canvas;

namespace Zine.App.Domain.ComicBook.Reading;

public class ReadingPageHandler
{
	public ReadingPageHandler(
		NavigationManager navigationManager,
		IComicBookService comicBookService,
		IJSRuntime jsRuntime,
		string canvasId,
		int groupId,
		int comicBookId,
		Action uiUpdateHandler)
	{
		_navigationManager = navigationManager;
		_comicBookService = comicBookService;
		_jsRuntime = jsRuntime;
		_uiUpdateHandler = uiUpdateHandler;

		LoadComic(comicBookId, groupId);
		LoadImages(comicBookId);
		_canvasHandler = new CanvasHandler(jsRuntime, canvasId);
	}

	public ComicBook ComicBook { get; private set; } = null!;

	public int ZoomScale = 100;

	private readonly IComicBookService _comicBookService;

	private readonly CanvasHandler _canvasHandler;

	private readonly NavigationManager _navigationManager;

	private readonly IJSRuntime _jsRuntime;

	private readonly Action _uiUpdateHandler;

	public int CurrentPageIndex
	{
		get => _currentPageIndex;
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

	// ReSharper disable once RedundantDefaultMemberInitializer
	private int _currentPageIndex = 0;

	public string[] Images = [];


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
		await _canvasHandler.RotateRight();
	}

	public async Task RotateLeft()
	{
		await _canvasHandler.RotateLeft();
	}

	public async Task ZoomIn()
	{
		await _canvasHandler.ZoomIn();
		await UpdateZoomScale();
	}

	public async Task ZoomOut()
	{
		await _canvasHandler.ZoomOut();
		await UpdateZoomScale();
	}

	public async Task UpdateZoomScale()
	{
		ZoomScale = await _canvasHandler.GetZoomScale();
		_uiUpdateHandler();
	}

	public async void SetDotnetHelperReference(DotNetObjectReference<Components.Pages.Reading> dotNetObjectReference)
	{
		await _canvasHandler.SetDotnetHelperReference(dotNetObjectReference);
	}


	private void LoadComic(int comicBookId, int groupId)
	{
		var loadedComicBook = _comicBookService.GetById(comicBookId);

		if (loadedComicBook == null)
			_navigationManager.NavigateTo(PageManager.GetLibraryGroupLink(groupId));
		else
			ComicBook = loadedComicBook;
	}


	private void LoadImages(int comicBookId)
	{
		_comicBookService.ExtractImagesOfComicBook(comicBookId);

		Images = Directory
			.GetFiles(DataPath.ComicBookReadingDirectory, "*.*", SearchOption.TopDirectoryOnly)
			.Select(path => "/images/Reading/" + Path.GetFileName(path))
			.Order()
			.ToArray();
	}

	private void SetImageOnCanvas(int imageIndex)
	{
		_ = _canvasHandler.DrawImage(Images[imageIndex]);
	}

	private void ScrollImageToViewInSidebar()
	{
		_jsRuntime.InvokeVoidAsync("scrollElementIntoView", "image-" + CurrentPageIndex);
	}

}
