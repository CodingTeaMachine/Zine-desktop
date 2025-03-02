using System.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Domain.ComicBookPageInformation;
using Zine.App.Helpers;
using Zine.App.Helpers.Canvas;

namespace Zine.App.Domain.ComicBook.Reading;

public class ReadingPageHandler
{
	public ReadingPageHandler(
		NavigationManager navigationManager,
		IComicBookService comicBookService,
		IComicBookInformationService comicBookInformationService,
		IJSRuntime jsRuntime,
		string canvasId,
		int groupId,
		int comicBookId,
		Action uiUpdateHandler)
	{
		_comicBookService = comicBookService;
		_jsRuntime = jsRuntime;
		_uiUpdateHandler = uiUpdateHandler;
		_canvasHandler = new CanvasHandler(jsRuntime, canvasId);

		try
		{
			LoadComic(comicBookId);
			comicBookInformationService.UpdateLastReadTimeToCurrentTime(ComicBook.Information.Id);
		}
		catch (DataException)
		{
			navigationManager.NavigateTo(PageManager.GetLibraryGroupLink(groupId));
		}

	}

	private ComicBook ComicBook { get; set; } = null!;

	public int ZoomScale = 100;

	private readonly IComicBookService _comicBookService;

	private readonly CanvasHandler _canvasHandler;

	private readonly IJSRuntime _jsRuntime;

	private readonly Action _uiUpdateHandler;

	public ComicBookPageInformation.ComicBookPageInformation CurrentPage => ComicBook.Pages.ToList()[_currentPageIndex];

	public int MaxPageNumber => ComicBook.Pages.ToList().Last().PageNumberEnd;

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

	public List<string> Images = [];


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


	private void LoadComic(int comicBookId)
	{
		var loadedComicBook = _comicBookService.GetForReadingView(comicBookId);

		if(loadedComicBook == null)
			throw new DataException("Comic book not found");

		ComicBook = loadedComicBook;

		LoadImages(comicBookId);
	}


	private void LoadImages(int comicBookId)
	{
		_comicBookService.ExtractImagesOfComicBook(comicBookId);

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
		_ = _canvasHandler.DrawImage(Images[imageIndex]);
	}

	private void ScrollImageToViewInSidebar()
	{
		_jsRuntime.InvokeVoidAsync("scrollElementIntoView", "image-" + CurrentPageIndex);
	}

}
