using System.Data;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using SharpCompress;
using SharpCompress.Archives;
using Zine.App.Database;
using Zine.App.Domain.ComicBook;
using Zine.App.Exceptions;
using Zine.App.Helpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBookPageInformation;

public class ComicBookPageInformationService(
	ZineDbContext dbContext,
	GenericRepository<ComicBookPageInformation> repository) : IComicBookPageInformationService
{
	public IEnumerable<ComicBookPageInformation> CreateMany(string comicBookPathOnDisk, int comicBookId)
	{
		var pagesToCreate = PageInfoListFactory.GetPageInfoList(comicBookPathOnDisk, comicBookId);

		try
		{
			repository.InsertMany(pagesToCreate);
			dbContext.SaveChanges();
			return pagesToCreate;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Failed to create comic book page informations", Severity.Error, e);
		}
	}

	public void CheckPageTypes(ComicBook.ComicBook comicBook)
	{
		var pageInfoHelper = new PageInfoHelper(comicBook.Pages);

		var comicBookFile = ArchiveFactory.Open(comicBook.FileUri);
		var coverPage = pageInfoHelper.GetCover();
		var coverImage = pageInfoHelper.GetFileFromArchiveByPage(comicBookFile, coverPage.PageFileName);

		float coverAspectRatio = Image.GetAspectRatio(coverImage);
		double minAspectRatio = coverAspectRatio * 0.7;


		var pageNumber = 1;

		if (pageInfoHelper.GetCoverInside() != null)
			pageNumber++;

		foreach (var page in pageInfoHelper.GetPages())
		{
			if(page.IsWidthChecked)
				continue;

			var pageAsArchiveEntry = pageInfoHelper.GetFileFromArchiveByPage(comicBookFile, page.PageFileName);
			var isDoubleImage = IsDoubleImage(pageAsArchiveEntry, minAspectRatio);

			page.PageType = isDoubleImage ? PageType.Double : PageType.Single;
			page.PageNumberStart = pageNumber;

			pageNumber += isDoubleImage ? 2 : 1;

			page.IsWidthChecked = true;

			repository.Update(page);
		}

		var backCoverInsideImage = pageInfoHelper.GetBackCoverInside();
		if (backCoverInsideImage != null && backCoverInsideImage.PageNumberStart != pageNumber)
		{
			pageNumber += 1;
			backCoverInsideImage.PageNumberStart = pageNumber;

			repository.Update(backCoverInsideImage);
		}

		var backCoverImage = pageInfoHelper.GetBackCover();
		if (backCoverImage != null && backCoverImage.PageNumberStart != pageNumber)
		{
			pageNumber += 1;
			backCoverImage.PageNumberStart = pageNumber;
			repository.Update(backCoverImage);
		}

		dbContext.SaveChanges();
		comicBookFile.Dispose();
	}

	public void UpdateReadStatus(int comicBookPageInformationId)
	{
		var pageToUpdate = repository.GetById(comicBookPageInformationId);

		if(pageToUpdate == null)
			throw new HandledAppException("Did not find comic book page information", Severity.Error, $"Could not find comic book page information by id: ${comicBookPageInformationId}");

		pageToUpdate.IsRead = true;

		try
		{
			repository.Update(pageToUpdate);
			dbContext.SaveChanges();
		}
		catch (Exception e)
		{
			throw new HandledAppException("Failed to update comic book page information", Severity.Error, e);
		}

	}

	private static bool IsDoubleImage(IArchiveEntry page, double minAspectRatio)
	{
		try
		{
			return Image.GetAspectRatio(page) <= minAspectRatio;
		}
		catch (DataException)
		{
			return false;
		}
	}
}
