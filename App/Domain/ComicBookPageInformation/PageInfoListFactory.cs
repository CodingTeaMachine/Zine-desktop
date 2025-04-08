using System.Text.RegularExpressions;
using SharpCompress;
using SharpCompress.Archives;
using Zine.App.Domain.ComicBookInformation;
using Zine.App.Helpers;

namespace Zine.App.Domain.ComicBookPageInformation;

public static class PageInfoListFactory
{
	public static List<ComicBookPageInformation> GetPageInfoList(string comicBookPathOnDisk, int comicBookId)
	{
		using IArchive comicBookFile = ArchiveFactory.Open(comicBookPathOnDisk);
		var infoExtractor = new CompressedComicBookPageInformationExtractor();

		List<IArchiveEntry> pages = infoExtractor
			.GetPages(comicBookFile)
			.OrderBy(entry => entry.Key)
			.ToList();

		var pageInfoList = new List<ComicBookPageInformation>();
		var pageIndex = 0;

		//Create the cover image
		var coverKvp = GetCoverImage(pages);
		pageInfoList.Add(new ComicBookPageInformation
		{
			PageFileName = coverKvp.Value.Key!,
			PageType = PageType.Cover,
			Index = pageIndex++,
			ComicBookId = comicBookId,
			IsWidthChecked = true
		});

		//Create the inside of the cover image
		string? coverInside = GetCoverInsideImage(pages, coverKvp.Key);
		if (coverKvp.Key != PageNamingConvention.Enumeration && coverInside != null)
		{
			pageInfoList.Add(new ComicBookPageInformation
			{
				PageFileName = coverInside,
				PageType = PageType.CoverInside,
				Index = pageIndex++,
				ComicBookId = comicBookId,
				IsWidthChecked = true
			});
		}

		//Create the back cover
		var backCover = GetBackCoverImage(pages, coverKvp.Key);
		string? backCoverInside = null;
		if (backCover.HasValue)
		{
			backCoverInside = GetBackCoverInsideImage(pages, backCover.Value.Key);
		}


		//The rest of the pages, already ordered in
		var coverImageFileName = coverKvp.Value.Key;
		var insidePages = pages.Where(entry =>
			{
				var fileName = entry.Key;
				return fileName != coverImageFileName &&
				       fileName != coverInside &&
				       fileName != backCover?.Value &&
				       fileName != backCoverInside;
			}
		);

		pageInfoList.AddRange(insidePages.Select(page => new ComicBookPageInformation
		{
			PageFileName = page.Key!,
			// Calculating weather the image is double with takes a lot of time (approx 29x longer)
			// so it is only calculated when the comic is first opened
			PageType = PageType.Single,
			Index = pageIndex++,
			ComicBookId = comicBookId,
			IsWidthChecked = false
		}));

		//Add the back cover, and it's inside to the end of it all
		if (backCover.HasValue)
		{
			if (backCoverInside != null)
			{
				pageInfoList.Add(new ComicBookPageInformation
				{
					PageFileName = backCoverInside,
					PageType = PageType.BackCoverInside,
					Index = pageIndex++,
					ComicBookId = comicBookId,
					IsWidthChecked = true
				});
			}

			pageInfoList.Add(new ComicBookPageInformation
			{
				PageFileName = backCover.Value.Value,
				PageType = PageType.BackCover,
				Index = pageIndex++,
				ComicBookId = comicBookId,
				IsWidthChecked = true
			});
		}

		return pageInfoList;
	}

	private static KeyValuePair<PageNamingConvention, IArchiveEntry> GetCoverImage(List<IArchiveEntry> pages)
	{
		foreach (var format in ComicBookPageNamingFormat.CoverPageFormatToRegexDic)
		{
			foreach (var page in pages.Where(page =>
				         Regex.IsMatch(Path.GetFileNameWithoutExtension(page.Key!), format.Value)))
			{
				return new KeyValuePair<PageNamingConvention, IArchiveEntry>(format.Key, page);
			}
		}

		return new KeyValuePair<PageNamingConvention, IArchiveEntry>(PageNamingConvention.Enumeration, pages.First());
	}

	private static string? GetCoverInsideImage(List<IArchiveEntry> pages,
		PageNamingConvention coverImageNamingConvention)
	{
		if (coverImageNamingConvention == PageNamingConvention.Enumeration)
		{
			return null;
		}

		var format = ComicBookPageNamingFormat.CoverInsidePageFormatToRegexDic[coverImageNamingConvention];
		var explicitFormat =
			ComicBookPageNamingFormat.CoverInsidePageFormatToRegexDic[PageNamingConvention.CoverInsideExplicit];

		return pages
			.Where(page =>
				Regex.IsMatch(Path.GetFileNameWithoutExtension(page.Key!), format) ||
				Regex.IsMatch(Path.GetFileNameWithoutExtension(page.Key!), explicitFormat)
			)
			.Select(page => page.Key!)
			.FirstOrDefault();
	}

	private static KeyValuePair<PageNamingConvention, string>? GetBackCoverImage(List<IArchiveEntry> pages,
		PageNamingConvention coverImageNamingConvention)
	{
		if (coverImageNamingConvention == PageNamingConvention.Enumeration)
		{
			return null;
		}

		var formatKey = coverImageNamingConvention == PageNamingConvention.CoverMarkedZeroBased
			? PageNamingConvention.CoverMarkedOneBased
			: PageNamingConvention.CoverMarkedTwoBased;

		var format = ComicBookPageNamingFormat.BackCoverPageFormatToRegexDic[formatKey];

		foreach (var page in pages.Where(page => Regex.IsMatch(Path.GetFileNameWithoutExtension(page.Key!), format)))
		{
			return new KeyValuePair<PageNamingConvention, string>(formatKey, page.Key!);
		}

		return null;
	}

	private static string? GetBackCoverInsideImage(List<IArchiveEntry> pages,
		PageNamingConvention backCoverImageNamingConvention)
	{
		var formatKey = backCoverImageNamingConvention == PageNamingConvention.CoverMarkedZeroBased
			? PageNamingConvention.CoverMarkedOneBased
			: PageNamingConvention.CoverMarkedTwoBased;

		var format = ComicBookPageNamingFormat.BackCoverInsidePageFormatToRegexDic[formatKey];
		var explicitFormat =
			ComicBookPageNamingFormat.BackCoverInsidePageFormatToRegexDic[PageNamingConvention.BackCoverInsideExplicit];

		return pages
			.Where(page =>
				Regex.IsMatch(Path.GetFileNameWithoutExtension(page.Key!), format) ||
				Regex.IsMatch(Path.GetFileNameWithoutExtension(page.Key!), explicitFormat)
			)
			.Select(page => page.Key!)
			.FirstOrDefault();
	}
}
