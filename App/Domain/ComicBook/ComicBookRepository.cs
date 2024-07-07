using Microsoft.EntityFrameworkCore;
using Zine.App.Database;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook;

public class ComicBookRepository(
	IDbContextFactory<ZineDbContext> contextFactory,
	ILoggerService logger)
	: Repository(contextFactory), IComicBookRepository
{
	public IEnumerable<ComicBook> GetAllByGroupId(int? groupId = null)
	{
		return GetDbContext().ComicBooks.Where(c => c.GroupId == groupId).Include(cb => cb.Information).ToList();
	}

	public ComicBook? GetById(int comicId)
	{
		return GetDbContext().ComicBooks.FirstOrDefault(c => c.Id == comicId);
	}

	public ComicBook Create(string name, string fileUri, ComicBookInformation.ComicBookInformation cbInfo, int? groupId = null )
	{
		var comicBookToCreate = new ComicBook { Name = name, FileUri = fileUri, GroupId = groupId, Information = cbInfo};

		var dbContext = GetDbContext();
		var createdComicBook = dbContext.ComicBooks.Add(comicBookToCreate);

		try
		{
			dbContext.SaveChanges();
		}
		catch (DbUpdateException e)
		{
			logger.Error($"ComicBookRepository.Create: Error saving comic to DB: {name} \n {e.Message}");
			throw;
		}

		return createdComicBook.Entity;
	}

	public void CreateMany(IEnumerable<ComicBook> comicBooks)
	{
		var dbContext = GetDbContext();
		dbContext.ComicBooks.AddRange(comicBooks);
		var createdComicBooks = dbContext.SaveChanges();

		logger.Information($"ComicBookRepository.CreateMany: Created {createdComicBooks} comic books");
	}

	public bool AddToGroup(int? groupId, int comicBookId)
	{
		var comicBook = GetById(comicBookId);
		comicBook!.GroupId = groupId;
		var updatedLines = GetDbContext().SaveChanges();
		var updateSuccessful =  updatedLines == 1;

		if (updateSuccessful)
			logger.Information($"ComicBookRepository.AddToGroup: Added {comicBookId} - {comicBook.Name} to {groupId}");
		else
			logger.Error($"ComicBookRepository.AddToGroup: Failed to add comic book ({comicBookId} - {comicBook.Name}) to {groupId}");

		return updateSuccessful;
	}

	public void MoveAll(int currentGroupId, int? newGroupId)
	{
		var context = GetDbContext();
		context.ComicBooks
			.Where(cb => cb.GroupId == currentGroupId)
			.ToList()
			.ForEach(cb => cb.GroupId = newGroupId);

		var updatedLines = context.SaveChanges();
		var newParent = newGroupId == null ? "root" : newGroupId.ToString();
		logger.Information($"ComicBookRepository.MoveAll: Moved {updatedLines} comic books  from: {currentGroupId} to: {newParent}");
	}

	public bool Rename(int comicBookId, string newName)
	{
		var comicBook = GetById(comicBookId);

		var oldComicBookName = comicBook!.Name;

		comicBook.Name = newName;
		var updatedLines = GetDbContext().SaveChanges();
		var updateSuccessful = updatedLines == 1;

		if (updateSuccessful)
			logger.Information($"ComicBookRepository.Rename: Renamed comic book from: {oldComicBookName} to: {newName} (id: {comicBookId})");
		else
			logger.Error($"ComicBookRepository.Rename: Failed to rename comic book: {comicBookId} to: {newName}");

		return updateSuccessful;
	}

	public void DeleteAllFromGroup(int groupId)
	{
		var updatedLines = GetDbContext().ComicBooks.Where(cb => cb.GroupId == groupId).ExecuteDelete();
		logger.Information($"ComicBookRepository.DeleteAllFromGroup: deleted {updatedLines} comic books");
	}
}
