namespace Zine.App.Domain.ComicBookInformation;

public static class ComicBookPageNamingFormat
{
	private const string CTaggedRegex = "c( ?)(0{1,2})(1{1})$";
	private const string CTaggedZeroBasedRegex = "c( ?)(0{1,2})(0{1})$";

	public static readonly Dictionary<ComicBookPageNamingFormatName, string> PageFormatToRegexDic =
		new()
		{
			{ ComicBookPageNamingFormatName.CTagged, CTaggedRegex },
			{ ComicBookPageNamingFormatName.CTaggedZeroBased, CTaggedZeroBasedRegex }
		};
}
