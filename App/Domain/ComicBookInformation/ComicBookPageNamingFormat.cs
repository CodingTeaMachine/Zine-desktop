namespace Zine.App.Domain.ComicBookInformation;

public static class ComicBookPageNamingFormat
{
	private const string CTagged = "c( ?)(0{1,2})(1{1})$";
	private const string CTaggedZeroBased = "c( ?)(0{1,2})(0{1})$";

	public static readonly Dictionary<ComicBookPageNamingFormatName, string> PageFromatToRegexDic =
		new()
		{
			{ComicBookPageNamingFormatName.CTagged, CTagged},
			{ComicBookPageNamingFormatName.CTaggedZeroBased, CTaggedZeroBased},
		};
}
