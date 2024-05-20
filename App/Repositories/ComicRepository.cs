using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Zine.App.Database;
using Zine.App.Enums;
using Zine.App.Factories;
using Zine.App.Logger;
using Zine.App.Model;

namespace Zine.App.Repositories;

public class ComicRepository(
    IDbContextFactory<ZineDbContext> contextFactory,
    ISettingsRepository settingsRepository,
    ILoggerService logger)
    : Repository(contextFactory), IComicRepository
{
    public IEnumerable<Comic> GetAll()
    {
        string? comicBookDirectory = settingsRepository.GetByKey(SettingsKeys.ComicBookPath)?.Value;

        if (comicBookDirectory is null or "")
        {
            logger.Warning("Comic book directory not set");
            return Array.Empty<Comic>();
        }

        //TODO: Store the result in a cache and load it from there once this ran once

        return GetAllFromDisc(comicBookDirectory);
    }

    private IEnumerable<Comic> GetAllFromDisc(string comicBookDirectory)
    {
        List<Comic> comics = Directory.EnumerateFiles(comicBookDirectory, "*.cb?", SearchOption.AllDirectories)
            .Select<string, string[]>(filename => [Path.GetFileNameWithoutExtension(filename), Path.GetExtension(filename), GetFilePathWithOutConstantPath(comicBookDirectory, filename)])
            .Where(fileProps => ComicFormatFactory.ComicFileExtensions.Contains(fileProps[1]))
            .Select(fileProps =>
                new Comic
                {
                    Name = fileProps[0],
                    Extension = fileProps[1],
                    FullPath = fileProps[2],
                    Format = ComicFormatFactory.GetFromFileExtension(fileProps[1])
                })
            .OrderBy(comic => comic.Name)
            .ToList();

        logger.Information($"Read {comics.Count} comics from: {comicBookDirectory}");

        return comics;
    }

    private string GetFilePathWithOutConstantPath(string constPath, string path)
    {
        string directory = Path.GetDirectoryName(path)!;
        int index = directory.IndexOf(constPath, StringComparison.Ordinal);

        //Here it is 100% that the path contains constpath
        return directory.Remove(index, constPath.Length);
    }
}
