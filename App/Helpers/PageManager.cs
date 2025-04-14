using System.Text;
using Zine.App.Enums;

namespace Zine.App.Helpers;

public static class PageManager
{
    private static readonly Dictionary<string, string> Pages = new()
    {
        {Page.Home, "Home"},
        {Page.Reading, "Reading"},
        {Page.Library, "Library"},
        {Page.Settings, "settings"},
    };

    public static string PageTitle { get; private set; } = Pages[Page.Home];

    public static void SetPageTitle(string path)
    {
        PageTitle = Pages.TryGetValue(path, value: out var title)
            ? title
            : "Home";
    }

    public static string GetLibraryGroupLink(int groupId)
    {
        return new StringBuilder(Page.Library)
            .Append('/')
            .Append(groupId)
            .ToString();
    }

    public static string GetReadingPageLink(int comicBookId, int groupId)
    {
        return new StringBuilder(Page.Reading)
            .Append('/')
            .Append(comicBookId)
            .Append('/')
            .Append(groupId)
            .ToString();
    }

}
