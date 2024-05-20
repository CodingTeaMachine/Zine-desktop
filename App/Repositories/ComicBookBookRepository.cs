using Microsoft.EntityFrameworkCore;
using Zine.App.Database;
using Zine.App.Factories;
using Zine.App.FileHelpers;
using Zine.App.Logger;
using Zine.App.Model;

namespace Zine.App.Repositories;

public class ComicBookBookRepository(
	IDbContextFactory<ZineDbContext> contextFactory,
	ILoggerService logger)
	: Repository(contextFactory), IComicBookRepository
{
	public IEnumerable<Comic> GetAll(){

		return GetAllFromDisk();
	}

	private IEnumerable<Comic> GetAllFromDisk()
	{
		List<Comic> comics = Directory.EnumerateFiles("ComicBookLinks")
			.Select<string, string[]>(filename =>
			[
				Path.GetFileNameWithoutExtension(filename),
				Path.GetExtension(filename),
			])
			.Where(fileProps => ComicFormatFactory.ComicFileExtensions.Contains(fileProps[1]))
			.Select(fileProps =>
				new Comic
				{
					Name = fileProps[0],
					Extension = fileProps[1],
					Format = ComicFormatFactory.GetFromFileExtension(fileProps[1])
				})
			.OrderBy(comic => comic.Name)
			.ToList();

		logger.Information($"Read {comics.Count} comics");

		return comics;
	}
}
