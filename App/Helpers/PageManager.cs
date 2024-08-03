using System.Text;
using Microsoft.Extensions.Primitives;
using Zine.App.Enums;

namespace Zine.App.Helpers;

public static class PageManager
{
    private static readonly Dictionary<string, string> Pages = new()
    {
        {Page.Home, "Home"},
        {Page.Reading, "Reading"},
        {Page.Library, "Library"}
    };

    public static string PageTitle { get; private set; } = Pages[Page.Home];

    public static void SetPageTitle(string path)
    {
        PageTitle = Pages.TryGetValue(path, value: out var title)
            ? title
            : "Home";
    }

    public static string GetLibraryGroupLink(int? groupId)
    {
        var sb = new StringBuilder(Page.Library);

        if (groupId != null)
            sb.Append('/').Append(groupId);

        return sb.ToString();
    }

    public static string GetLibraryComicBookLink(int comicBookId, int? groupId)
    {
        var sb = new StringBuilder(Page.Reading)
            .Append('/')
            .Append(comicBookId);

        if (groupId != null)
            sb.Append('/').Append(groupId);

        return sb.ToString();
    }
}
