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
		const double aspectRatio = 0.7;

		var pageInfoHelper = new PageInfoHelper(comicBook.Pages);

		var comicBookFile = ArchiveFactory.Open(comicBook.FileUri);
		var coverPage = pageInfoHelper.GetCover();
		var coverImage = pageInfoHelper.GetFileFromArchiveByPage(comicBookFile, coverPage.PageFileName);

		float coverAspectRatio = Image.GetAspectRatio(coverImage);
		double minAspectRatio = coverAspectRatio * aspectRatio;

		var pages = pageInfoHelper
			.GetPages()
			.Where(page => page.IsWidthChecked == false);

		foreach (var page in pages)
		{
			if(page.IsWidthChecked)
				continue;

			var pageAsArchiveEntry = pageInfoHelper.GetFileFromArchiveByPage(comicBookFile, page.PageFileName);
			var isDoubleImage = IsDoubleImage(pageAsArchiveEntry, minAspectRatio);
			page.PageType = isDoubleImage ? PageType.Double : PageType.Single;
			page.IsWidthChecked = true;
			repository.Update(page);
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

	public void UpdateMany(List<ComicBookPageInformation> pages)
	{
		repository.UpdateMany(pages);
		dbContext.SaveChanges();
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
