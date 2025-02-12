using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Database;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook;

public class ComicBookRepository(
	IDbContextFactory<ZineDbContext> contextFactory,
	ILoggerService logger)
	: IComicBookRepository
{

	//Read

	/// <summary>
	///
	/// </summary>
	/// <param name="groupId"></param>
	/// <returns></returns>
	/// <exception cref="HandledAppException"></exception>
	public IEnumerable<ComicBook> GetAllByGroupId(int groupId)
	{
		try
		{
			logger.Information($"Getting all comic books by groupId \"{groupId}\"");
			using var context = contextFactory.CreateDbContext();
			return context.ComicBooks.Where(c => c.GroupId == groupId).Include(cb => cb.Information).ToList();
		}
		catch (Exception e)
		{
			throw new HandledAppException("Error loading comic books", Severity.Error, e);
		}
	}

/// <summary>
///
/// </summary>
/// <param name="comicId"></param>
/// <returns></returns>
/// <exception cref="HandledAppException"></exception>
	public ComicBook? GetById(int comicId)
	{
		try
		{
			using var context = contextFactory.CreateDbContext();
			return context.ComicBooks.Include(c => c.Information).FirstOrDefault(c => c.Id == comicId);
		}
		catch (Exception e)
		{
			throw new HandledAppException("Error loading comic books", Severity.Error, e);
		}
	}


	public IEnumerable<ComicBook> SearchByTitle(string searchTerm)
	{
		searchTerm = searchTerm.ToLower();
		logger.Information($"ComicBookRepository.SearchByTitle: Searching for comic books by title: \"{searchTerm}\"");

		try
		{
			using var context = contextFactory.CreateDbContext();
			var comicBooksFoundByTitle =  context.ComicBooks.Where(cb => cb.Title.ToLower().Contains(searchTerm));

			logger.Information($"ComicBookRepository.SearchByTitle: Found {comicBooksFoundByTitle.Count()} comic books for term \"{searchTerm}\"");

			return comicBooksFoundByTitle;
		}
		catch (Exception e)
		{
			throw new HandledAppException($"Error searching comic books by title: \"{searchTerm}\"", Severity.Error, e);
		}

	}

	//Create

	/// <summary>
	///
	/// </summary>
	/// <param name="title"></param>
	/// <param name="fileUri"></param>
	/// <param name="groupId"></param>
	/// <param name="context"></param>
	/// <returns></returns>
	public ComicBook Create(string title, string fileUri, int groupId, ZineDbContext? context = null)
	{

		var comicBookToCreate = new ComicBook
			{ Title = title, FileUri = fileUri, GroupId = groupId };


		context ??= contextFactory.CreateDbContext();

		var createdComicBook = context.ComicBooks.Add(comicBookToCreate);

		try
		{
			context.SaveChanges();
			logger.Information($"Created comic book: \"{title}\"");
			return createdComicBook.Entity;
		}
		catch (DbUpdateException e)
		{
			//TODO: Error handling
			Console.WriteLine(e);
			throw;
		}
	}

/// <summary>
///
/// </summary>
/// <param name="comicBooks"></param>
/// <exception cref="HandledAppException"></exception>
	public void CreateMany(IEnumerable<ComicBook> comicBooks)
	{
		var comicBooksAsArray = comicBooks.ToArray();

		using var context = contextFactory.CreateDbContext();
		context.ComicBooks.AddRange(comicBooksAsArray);

		try
		{
			var createdComicBooks = context.SaveChanges();
			logger.Information($"ComicBookRepository.CreateMany: Created {createdComicBooks} / {comicBooksAsArray.Length} comic books");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Could save comic books", Severity.Warning, e);
		}
	}

	//Update

	/// <summary>
	///
	/// </summary>
	/// <param name="groupId"></param>
	/// <param name="comicBookId"></param>
	/// <returns></returns>
	/// <exception cref="HandledAppException"></exception>
	public void AddToGroup(int groupId, int comicBookId)
	{

		using var context = contextFactory.CreateDbContext();
		try
		{
			var comicBook = context.ComicBooks.Include(cb => cb.Group).First(cb => cb.Id == comicBookId);
			var group = context.Groups.First(g => g.Id == groupId);

			comicBook.Group = group;

			var updatedLines = context.SaveChanges();

			if (updatedLines == 1)
				logger.Information($"ComicBookRepository.AddToGroup: Added {comicBookId} - {comicBook.Title} to {groupId}");
			else
				throw new HandledAppException($"Failed to add comic book ({comicBookId} - {comicBook.Title}) to {groupId}",
					Severity.Warning);
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error adding comic book to group", Severity.Error, e);
		}
	}

	/// <summary>
///
/// </summary>
/// <param name="currentGroupId"></param>
/// <param name="newGroupId"></param>
/// <exception cref="HandledAppException"></exception>
	public void MoveAll(int currentGroupId, int newGroupId)
	{
		var context = contextFactory.CreateDbContext();
		context.ComicBooks
			.Where(cb => cb.GroupId == currentGroupId)
			.ToList()
			.ForEach(cb => cb.GroupId = newGroupId);
		try
		{
			var updatedLines = context.SaveChanges();
			logger.Information(
				$"ComicBookRepository.MoveAll: Moved {updatedLines} comic books  from: {currentGroupId} to: {newGroupId.ToString()}");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error moving comic books to group", Severity.Error, e);
		}
	}

/// <summary>
///
/// </summary>
/// <param name="comicBookId"></param>
/// <param name="newName"></param>
/// <returns></returns>
/// <exception cref="HandledAppException"></exception>
	public void Rename(int comicBookId, string newName)
	{
		using var context = contextFactory.CreateDbContext();
		var comicBook = context.ComicBooks.First(c => c.Id == comicBookId);
		var oldComicBookName = comicBook.Title;
		comicBook.Title = newName;

		try
		{
			var updatedLines = context.SaveChanges();
			var updateSuccessful = updatedLines == 1;

			if (updateSuccessful)
				logger.Information($"ComicBookRepository.Rename: Renamed comic book from: {oldComicBookName} to: {newName} (id: {comicBookId})");
			else
				throw new DbUpdateException($"Error renaming comic book from: '{oldComicBookName}' to: '{newName}' ({comicBookId} - updatedLines:{updatedLines})");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Could not rename comic book", Severity.Warning, e);
		}
	}

	//Delete


/// <summary>
///
/// </summary>
/// <param name="comicId"></param>
/// <returns></returns>
/// <exception cref="HandledAppException"></exception>
	public bool Delete(int comicId)
	{
		using var context = contextFactory.CreateDbContext();
		var comicToDelete = context.ComicBooks
			.Include(comicBook => comicBook.Information)
			.First(cb => cb.Id == comicId);

		logger.Information($"ComicBookService.Delete: Deleting cover image for: {comicToDelete.Id} at {comicToDelete.Information.SavedCoverImageFullPath}");
		File.Delete(comicToDelete.Information.SavedCoverImageFullPath);

		context.ComicBooks.Remove(comicToDelete);
		try
		{
			var updatedLines = context.SaveChanges();
			var updateSuccessful = updatedLines != 0;

			if(!updateSuccessful)
				throw new DbUpdateException($"Failed to delete comic book: {comicId}");

			logger.Information($"ComicBookRepository.Delete: Deleted comic book: {comicId}");
			return updateSuccessful; //TODO: Don't return anything
		}
		catch (Exception e)
		{
			throw new HandledAppException("Error deleting comic book", Severity.Warning, e);
		}

	}

	/// <summary>
	///
	/// </summary>
	/// <param name="groupId"></param>
	/// <exception cref="HandledAppException"></exception>
	public void DeleteAllFromGroup(int groupId)
	{
		try
		{
			using var context = contextFactory.CreateDbContext();
			var comicBooksToDelete = GetAllByGroupId(groupId);

			comicBooksToDelete.ToList().ForEach(cb =>
			{
				logger.Information($"ComicBookRepository.DeleteAllFromGroup: Deleting cover image for: {cb.Id} at {cb.Information.SavedCoverImageFullPath}");
				File.Delete(cb.Information.SavedCoverImageFullPath);
				context.ComicBooks.Remove(cb);
			});


			var updatedLines = context.SaveChanges();
			logger.Information($"ComicBookRepository.DeleteAllFromGroup: deleted {updatedLines / 2} comic books");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error delete comic books from group", Severity.Warning, e);
		}
	}
}
