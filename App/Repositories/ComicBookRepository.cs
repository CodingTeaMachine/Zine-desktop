using Microsoft.EntityFrameworkCore;
using Zine.App.Database;
using Zine.App.Logger;
using Zine.App.Model.DB;

namespace Zine.App.Repositories;

public class ComicBookRepository(
	IDbContextFactory<ZineDbContext> contextFactory,
	ILoggerService logger)
	: Repository(contextFactory), IComicBookRepository
{
	public IEnumerable<ComicBook> GetAllByGroupId(int? groupId = null)
	{
		return GetDbContext().ComicBooks.Where(c => c.GroupId == groupId).ToList();
	}

	public ComicBook? GetById(int comicId)
	{
		return GetDbContext().ComicBooks.FirstOrDefault(c => c.Id == comicId);
	}

	/// <summary>
	///		Creates a comicboob in the database
	/// </summary>
	/// <param name="name"></param>
	/// <param name="fileName"></param>
	/// <param name="groupId"></param>
	/// <exception cref="DbUpdateException">Throws when the new comic book could not be inserted to the db</exception>
	/// <returns></returns>
	public ComicBook Create(string name, string fileName, int? groupId = null)
	{
		var comicBookToCreate = new ComicBook { Name = name, FileName = fileName, GroupId = groupId};
		var dbContext = GetDbContext();
		var createdComicBook = dbContext.ComicBooks.Add(comicBookToCreate);

		try
		{
			dbContext.SaveChanges();
		}
		catch (DbUpdateException e)
		{
			logger.Error($"Error saving comic to DB: {name} \n {e.Message}");
			throw;
		}

		return createdComicBook.Entity;
	}

	public void CreateMany(IEnumerable<ComicBook> comicBooks)
	{
		var dbContext = GetDbContext();
		dbContext.ComicBooks.AddRange(comicBooks);
		dbContext.SaveChanges();
	}

	public bool AddToGroup(int? groupId, int targetId)
	{
		var comicBook = GetById(targetId);
		comicBook!.GroupId = groupId;
		GetDbContext().SaveChanges();
		//TODO: Error handling
		return true;
	}
}
