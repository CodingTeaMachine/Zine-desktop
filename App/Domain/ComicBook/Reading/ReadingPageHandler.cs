using Microsoft.AspNetCore.Components;
using Zine.App.Enums;
using Zine.App.Helpers;
using Zine.App.Helpers.Canvas;

namespace Zine.App.Domain.ComicBook.Reading;

public class ReadingPageHandler
{
	public ReadingPageHandler(
		NavigationManager navigationManager,
		IComicBookService comicBookService,
		CanvasHandler canvasHandler,
		int groupId,
		int comicBookId)
	{
		_navigationManager = navigationManager;
		_comicBookService = comicBookService;
		_canvasHandler = canvasHandler;

		LoadComic(comicBookId, groupId);
		LoadImages(comicBookId);
	}

	public ComicBook ComicBook { get; set; } = null!;

	public int ZoomScale = 100;

	private readonly IComicBookService _comicBookService;

	private readonly CanvasHandler _canvasHandler;

	private readonly NavigationManager _navigationManager;

	public int CurrentPageIndex
	{
		get => _currentPageIndex;
		private set
		{
			if (value != _currentPageIndex)
			{
				_currentPageIndex = value;
				SetImageOnCanvas(value);
			}
		}
	}

	private int _currentPageIndex = 0;

	public string[] Images = [];


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


	public void RefreshCanvasImage()
	{
		SetImageOnCanvas(CurrentPageIndex);
	}

	public async Task ZoomIn()
	{
		await _canvasHandler.ZoomIn();
		ZoomScale = await _canvasHandler.GetZoomScale();
	}

	public async Task ZoomOut()
	{
		await _canvasHandler.ZoomOut();
		ZoomScale = await _canvasHandler.GetZoomScale();
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

}
