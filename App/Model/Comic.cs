using Zine.App.Enums;

namespace Zine.App.Model;

public struct Comic
{
    public required string Name { get; set; }
    public required string Extension { get; set; }
    public required ComicFormat Format { get; set; }
}
