namespace Zine.App.Domain.ComicBookInformation;

public static class ComicBookPageNamingFormat
{
	private const string Cover_CTaggedRegex = "c( ?)(0{1,2})(1{1})$";
	private const string Cover_CTaggedZeroBasedRegex = "c( ?)(0{1,2})(0{1})$";

	private const string Rear_CTaggedRegex = "ic( ?)(0{1,2})(1{1})$";
	private const string Rear_CTaggedZeroBasedRegex = "ic( ?)(0{1,2})(0{1})$";

	public static readonly Dictionary<ComicBookPageNamingFormatName, string> CoverPageFormatToRegexDic =
		new()
		{
			{ ComicBookPageNamingFormatName.CTagged, Cover_CTaggedRegex },
			{ ComicBookPageNamingFormatName.CTaggedZeroBased, Cover_CTaggedZeroBasedRegex }
		};

	public static readonly Dictionary<ComicBookPageNamingFormatName, string> RearPageFormatToRegexDic =
		new()
		{
			{ ComicBookPageNamingFormatName.CTagged, Rear_CTaggedRegex },
			{ ComicBookPageNamingFormatName.CTaggedZeroBased, Rear_CTaggedZeroBasedRegex }
		};
}
