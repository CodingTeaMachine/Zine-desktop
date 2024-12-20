using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Database;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook;

public class ComicBookRepository(
	IDbContextFactory<ZineDbContext> contextFactory,
	ILoggerService logger)
	: Repository(contextFactory), IComicBookRepository
{
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
			return GetDbContext().ComicBooks.Where(c => c.GroupId == groupId).Include(cb => cb.Information).ToList();
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
			return GetDbContext().ComicBooks.Include(c => c.Information).FirstOrDefault(c => c.Id == comicId);
		}
		catch (Exception e)
		{
			throw new HandledAppException("Error loading comic books", Severity.Error, e);
		}
	}

/// <summary>
///
/// </summary>
/// <param name="title"></param>
/// <param name="fileUri"></param>
/// <param name="cbInfo"></param>
/// <param name="groupId"></param>
/// <returns></returns>
	public ComicBook Create(string title, string fileUri, ComicBookInformation.ComicBookInformation cbInfo, int groupId)
	{

		var comicBookToCreate = new ComicBook
			{ Title = title, FileUri = fileUri, GroupId = groupId, Information = cbInfo };

		var dbContext = GetDbContext();
		var createdComicBook = dbContext.ComicBooks.Add(comicBookToCreate);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"Created comic book: \"{title}\"");
			return createdComicBook.Entity;
		}
		catch (DbUpdateException e)
		{
			Console.WriteLine(e);
			throw;
		}
		finally
		{
			DisposeDbContext();
		}
	}

/// <summary>
///
/// </summary>
/// <param name="comicBooks"></param>
/// <exception cref="HandledAppException"></exception>
	public void CreateMany(IEnumerable<ComicBook> comicBooks)
	{
		try
		{
			var dbContext = GetDbContext();
			dbContext.ComicBooks.AddRange(comicBooks);
			var createdComicBooks = dbContext.SaveChanges();
			logger.Information($"ComicBookRepository.CreateMany: Created {createdComicBooks} / {comicBooks.Count()} comic books");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Could save comic books", Severity.Warning, e);
		}
		finally
		{
			DisposeDbContext();
		}

	}

/// <summary>
///
/// </summary>
/// <param name="groupId"></param>
/// <param name="comicBookId"></param>
/// <returns></returns>
/// <exception cref="HandledAppException"></exception>
	public void AddToGroup(int groupId, int comicBookId)
	{

		var dbContext = GetDbContext();
		try
		{
			var comicBook = dbContext.ComicBooks.Include(cb => cb.Group).First(cb => cb.Id == comicBookId);
			var group = dbContext.Groups.First(g => g.Id == groupId);

			comicBook.Group = group;

			var updatedLines = dbContext.SaveChanges();

			if (updatedLines == 1)
				logger.Information($"ComicBookRepository.AddToGroup: Added {comicBookId} - {comicBook.Title} to {groupId}");
			else
				throw new HandledAppException($"Failed to add comic book ({comicBookId} - {comicBook.Title}) to {groupId}");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error adding comic book to group", Severity.Error, e);
		}
		finally
		{
			DisposeDbContext();
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
		var context = GetDbContext();
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
		finally
		{
			DisposeDbContext();
		}
	}

/// <summary>
///
/// </summary>
/// <param name="comicBookId"></param>
/// <param name="newName"></param>
/// <returns></returns>
/// <exception cref="HandledAppException"></exception>
	public bool Rename(int comicBookId, string newName)
	{
		var comicBook = GetById(comicBookId);
		var oldComicBookName = comicBook!.Title;
		comicBook.Title = newName;
		try
		{
			var updatedLines = GetDbContext().SaveChanges();
			var updateSuccessful = updatedLines == 1;

			if (updateSuccessful)
				logger.Information(
					$"ComicBookRepository.Rename: Renamed comic book from: {oldComicBookName} to: {newName} (id: {comicBookId})");
			else
				throw new DbUpdateException($"Error renaming comic book from: {oldComicBookName} to: {newName} ({comicBookId} updatedLines:{updatedLines})");


			return updateSuccessful;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Could not rename comic book", Severity.Warning, e);
		}
		finally
		{
			DisposeDbContext();
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
			var updatedLines = GetDbContext().ComicBooks.Where(cb => cb.GroupId == groupId).ExecuteDelete();
			logger.Information($"ComicBookRepository.DeleteAllFromGroup: deleted {updatedLines / 2} comic books");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error delete comic books from group", Severity.Warning, e);
		}
		finally
		{
			DisposeDbContext();
		}

	}

/// <summary>
///
/// </summary>
/// <param name="comicId"></param>
/// <returns></returns>
/// <exception cref="HandledAppException"></exception>
	public bool Delete(int comicId)
	{
		var context = GetDbContext();
		var comicToDelete = context.ComicBooks.First(cb => cb.Id == comicId);
		context.ComicBooks.Remove(comicToDelete);
		try
		{
			var updatedLines = context.SaveChanges();
			var updateSuccessful = updatedLines != 0;

			if (updateSuccessful)
				logger.Information($"ComicBookRepository.Delete: Deleted comic book: {comicId}");
			else
				logger.Error($"ComicBookRepository.Delete: Failed to delete comic book: {comicId}");

			return updateSuccessful;
		}
		catch (Exception e)
		{
			throw new HandledAppException("Error delete comic book", Severity.Warning, e);
		}
		finally
		{
			DisposeDbContext();
		}
	}

	public IEnumerable<ComicBook> SearchByTitle(string searchTerm)
	{
		searchTerm = searchTerm.ToLower();
		logger.Information($"ComicBookRepository.SearchByTitle: Searching for comic books by title: \"{searchTerm}\"");

		try
		{
			var context = GetDbContext();
			var comicBooksFoundByTitle =  context.ComicBooks.Where(cb => cb.Title.ToLower().Contains(searchTerm));

			logger.Information($"ComicBookRepository.SearchByTitle: Found {comicBooksFoundByTitle.Count()} comic books for term \"{searchTerm}\"");

			return comicBooksFoundByTitle;
		}
		catch (Exception e)
		{
			throw new HandledAppException($"Error searching comic books by title: \"{searchTerm}\"", Severity.Error, e);
		}
		finally
		{
			DisposeDbContext();
		}
	}
}
