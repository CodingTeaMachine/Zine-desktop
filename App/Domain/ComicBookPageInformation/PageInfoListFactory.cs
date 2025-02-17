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
		var pageNumber = 1;

		//Create the cover image
		var coverKvp = GetCoverImage(pages);
		pageInfoList.Add(new ComicBookPageInformation
		{
			PageFileName = Path.GetFileName(coverKvp.Value.Key!),
			PageType = PageType.Cover,
			PageNumberStart = pageNumber++,
			ComicBookId = comicBookId,
		});

		//Create the inside of the cover image
		string? coverInside = GetCoverInsideImage(pages, coverKvp.Key);
		if (coverKvp.Key != PageNamingConvention.Enumeration && coverInside != null)
		{
			pageInfoList.Add(new ComicBookPageInformation
			{
				PageFileName = coverInside,
				PageType = PageType.CoverInside,
				PageNumberStart = pageNumber++,
				ComicBookId = comicBookId,
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
		var coverImageFileName = Path.GetFileName(coverKvp.Value.Key);
		var insidePages = pages.Where(entry =>
			{
				var fileName = Path.GetFileName(entry.Key);
				return fileName != coverImageFileName &&
				       fileName != coverInside &&
				       fileName != backCover?.Value &&
				       fileName != backCoverInside;
			}
		);

		float coverAspectRatio = Image.GetAspectRatio(coverKvp.Value);
		double minAspectRatio = coverAspectRatio * 0.7;

		insidePages.ForEach(page =>
		{
			var isDoubleImage = IsDoubleImage(page, minAspectRatio);

			pageInfoList.Add(new ComicBookPageInformation()
			{
				PageFileName = Path.GetFileName(page.Key!),
				PageType = isDoubleImage ? PageType.Double : PageType.Single,
				PageNumberStart = pageNumber,
				ComicBookId = comicBookId,
			});

			pageNumber += isDoubleImage ? 2 : 1;
		});

		//Add the back cover, and it's inside to the end of it all
		if (backCover.HasValue)
		{
			if (backCoverInside != null)
			{
				pageInfoList.Add(new ComicBookPageInformation
				{
					PageFileName = backCoverInside,
					PageType = PageType.BackCoverInside,
					PageNumberStart = pageNumber++,
					ComicBookId = comicBookId,
				});
			}

			pageInfoList.Add(new ComicBookPageInformation
			{
				PageFileName = backCover.Value.Value,
				PageType = PageType.BackCover,
				PageNumberStart = pageNumber++,
				ComicBookId = comicBookId,
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
			.Select(page => Path.GetFileName(page.Key!))
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
			return new KeyValuePair<PageNamingConvention, string>(formatKey, Path.GetFileName(page.Key!));
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
			.Select(page => Path.GetFileName(page.Key!))
			.FirstOrDefault();
	}

	private static bool IsDoubleImage(IArchiveEntry page, double minAspectRatio)
	{
		return Image.GetAspectRatio(page) <= minAspectRatio;
	}
}
