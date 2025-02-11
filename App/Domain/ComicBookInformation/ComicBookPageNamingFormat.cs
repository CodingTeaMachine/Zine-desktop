namespace Zine.App.Domain.ComicBookInformation;

public static class ComicBookPageNamingFormat
{
	private const string CoverZeroBased = "[^i]c([\\s_-])?(0+)?0$";
	private const string CoverOneBased = "[^i]c([\\s_-])?(0+)?1$";

	private const string CoverInsideZeroBased = "ic([\\s_-])?(0+)?0$";
	private const string CoverInsideOneBased = "ic([\\s_-])?(0+)?1$";
	private const string CoverInsideExplicit = "ifc$";

	private const string BackCoverOneBased = "[^i]c([\\s_-])?(0+)?1$";
	private const string BackCoverTwoBased = "[^i]c([\\s_-])?(0+)?2$";

	private const string BackCoverInsideOneBased = "ic([\\s_-])?(0+)?1$";
	private const string BackCoverInsideTwoBased = "ic([\\s_-])?(0+)?2$";
	private const string BackCoverInsideExplicit = "ibc$";

	public static readonly Dictionary<PageNamingConvention, string> CoverPageFormatToRegexDic =
		new()
		{
			{ PageNamingConvention.CoverMarkedZeroBased, CoverZeroBased },
			{ PageNamingConvention.CoverMarkedOneBased, CoverOneBased },
		};

	public static readonly Dictionary<PageNamingConvention, string> CoverInsidePageFormatToRegexDic =
		new()
		{
			{ PageNamingConvention.CoverMarkedZeroBased, CoverInsideZeroBased },
			{ PageNamingConvention.CoverMarkedOneBased, CoverInsideOneBased },
			{ PageNamingConvention.CoverInsideExplicit, CoverInsideExplicit }
		};

	public static readonly Dictionary<PageNamingConvention, string> BackCoverPageFormatToRegexDic =
		new()
		{
			{ PageNamingConvention.CoverMarkedOneBased, BackCoverOneBased },
			{ PageNamingConvention.CoverMarkedTwoBased, BackCoverTwoBased },
		};

	public static readonly Dictionary<PageNamingConvention, string> BackCoverInsidePageFormatToRegexDic =
		new()
		{
			{ PageNamingConvention.CoverMarkedOneBased, BackCoverInsideOneBased },
			{ PageNamingConvention.CoverMarkedTwoBased, BackCoverInsideTwoBased },
			{ PageNamingConvention.BackCoverInsideExplicit, BackCoverInsideExplicit }
		};
}
