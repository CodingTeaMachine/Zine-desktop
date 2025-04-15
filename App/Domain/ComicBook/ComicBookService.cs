using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using SharpCompress;
using SharpCompress.Archives;
using Zine.App.Database;
using Zine.App.Domain.ComicBook.DTO;
using Zine.App.Domain.ComicBookPageInformation;
using Zine.App.Enums;
using Zine.App.Exceptions;
using Zine.App.FileHelpers;
using Zine.App.Helpers;
using Zine.App.Logger;

namespace Zine.App.Domain.ComicBook;

public class ComicBookService(
	ZineDbContext dbContext,
	GenericRepository<ComicBook> repository,
	GenericRepository<Group.Group> groupRepository,
	IComicBookPageInformationService comicBookPageInformationService,
	ILoggerService logger
) : IComicBookService
{
	// Create

	public ComicBook Create(string title, string fileUri, int groupId)
	{
		var comicBookToCreate = new ComicBook
			{ Title = title, FileUri = fileUri, GroupId = groupId };


		repository.Insert(comicBookToCreate);

		try
		{
			dbContext.SaveChanges();
			logger.Information($"Created comic book: \"{title}\"");
			return comicBookToCreate;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException($"Failed to create comic book: \"{title}\"", Severity.Error, e);
		}
	}

	// Read

	public ComicBook? GetById(int comicId)
	{
		return repository.GetById(comicId);
	}

	public IEnumerable<ComicBook> GetCurrentlyReadComicBooks(int count)
	{
		return repository.List(
			includes: q => q.Include(cb => cb.Information),
			filter: cb => cb.Information.LastOpened != null,
			orderByDescending: cb => cb.Information.LastOpened ?? DateTime.MinValue,
			take: count);
	}

	public IEnumerable<ComicBook> GetRecommendations(int recommendationCount)
	{
		var totalCount = repository.Count(cb => cb.Information.LastOpened == null);

		List<ComicBook> recommendations = [];
		List<int> generatedIndexes = [];

		while (recommendations.Count < Math.Min(recommendationCount, totalCount))
		{
			var randomIndex = new Random().Next(0, totalCount - 1);

			if (generatedIndexes.Contains(randomIndex))
				continue;


			generatedIndexes.Add(randomIndex);

			var recommendation = repository.List(
				filter: cb => cb.Information.LastOpened == null,
				includes: q => q.Include(cb => cb.Information),
				skip: randomIndex,
				take: 1
			).First();


			recommendations.Add(recommendation);
		}

		return recommendations;
	}

	public ComicBook GetForReadingView(int comicId)
	{
		var comicBook = repository.First(
			filter: c => c.Id == comicId,
			includes: query =>
				query.Include(c => c.Pages)
					.Include(q => q.Information));

		if (comicBook == null)
			throw new HandledAppException("Could not find comic book", Severity.Error,
				"Could not find comic book: " + comicId);

		comicBookPageInformationService.CheckPageTypes(comicBook);

		return comicBook;
	}

	public ComicBook GetForInformationDrawer(int comicId)
	{
		var comicBook = repository.First(
				filter: c => c.Id == comicId,
				includes: query =>
					query
						.Include(c => c.Pages)
						.Include(c => c.Information)
						.ThenInclude(cbI => cbI.People)
						.Include(cb => cb.Information)
						.ThenInclude(cbI => cbI.Publishers)
						.Include(cb => cb.Information)
						.ThenInclude(cbI => cbI.Tags)
						.Include(cb => cb.Information)
						.ThenInclude(cbI => cbI.Series)!
			)
			;

		if (comicBook == null)
			throw new HandledAppException("Could not find comic book", Severity.Error,
				"Could not find comic book: " + comicId);

		return comicBook;
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="search"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public IEnumerable<ComicBook> Search(ComicBookSearchDto search)
	{
		try
		{
			var comicBooks = repository.List(
				searchQuery: search,
				includes: query =>
					query.Include(c => c.Pages)
						.Include(c => c.Information)
						.ThenInclude(cbI => cbI.People)
						.Include(cb => cb.Information)
						.ThenInclude(cbI => cbI.Publishers)
						.Include(cb => cb.Information)
						.ThenInclude(cbI => cbI.Tags)
						.Include(cb => cb.Information)
						.ThenInclude(cbI => cbI.Series)!
			).ToArray();

			logger.Information(
				$"ComicBookService.Search: Found {comicBooks.Length} comic books for term: \"{search}\"");

			return comicBooks;
		}
		catch (Exception e)
		{
			throw new HandledAppException("Error searching comic books", Severity.Error, e);
		}

	}

	public bool Any()
	{
		return repository.Any();
	}

	// Update

	public void AddToGroup(int groupId, int comicBookId)
	{
		try
		{
			var comicBook = repository.GetById(comicBookId);
			if (comicBook == null)
			{
				throw new HandledAppException($"Could not find comic with id: {comicBookId}", Severity.Warning);
			}

			var group = groupRepository.GetById(groupId);
			if (group == null)
			{
				throw new HandledAppException($"Could not find group with id: {groupId}", Severity.Warning);
			}

			comicBook.Group = group;

			dbContext.SaveChanges();
		}
		catch (DbUpdateException)
		{
			throw new HandledAppException("Could not update comic book", Severity.Error);
		}
	}

	public void MoveAll(int currentGroupId, int newGroupId)
	{
		repository
			.List(filter: cb => cb.GroupId == currentGroupId)
			.ForEach(cb =>
			{
				cb.GroupId = newGroupId;
				repository.Update(cb);
			});

		try
		{
			var updatedLines = dbContext.SaveChanges();
			logger.Information(
				$"ComicBookRepository.MoveAll: Moved {updatedLines} comic books  from: {currentGroupId} to: {newGroupId.ToString()}");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error moving comic books to group", Severity.Error, e);
		}
	}

	public void Rename(int comicBookId, string newTitle)
	{
		var comicBookToUpdate = repository.GetById(comicBookId);

		if (comicBookToUpdate == null)
			throw new HandledAppException("Could not find comic book", Severity.Warning);

		comicBookToUpdate.Title = newTitle;
		repository.Update(comicBookToUpdate);
		try
		{
			dbContext.SaveChanges();
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error renaming comic book", Severity.Error, e);
		}
	}

	public ComicBook Update(ComicBook comicBook)
	{
		repository.Update(comicBook);

		try
		{
			dbContext.SaveChanges();
			return comicBook;
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error updating comic book", Severity.Error, e);
		}
	}

	// Delete

	public void Delete(int comicId)
	{
		var comicToDelete =
			repository.First(cb => cb.Id == comicId, includes: query => query.Include(c => c.Information));

		if (comicToDelete == null)
			throw new HandledAppException("Could not find comic book", Severity.Error);

		if (File.Exists(comicToDelete.Information.SavedCoverImageFullPath))
		{
			File.Delete(comicToDelete.Information.SavedCoverImageFullPath);
		}

		repository.Delete(comicId);

		try
		{
			dbContext.SaveChanges();
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error deleting comic", Severity.Error, e);
		}
	}

	public void DeleteAllFromGroup(int groupId)
	{
		try
		{
			var comicBooksToDelete = repository.List(
				filter: cb => cb.GroupId == groupId,
				includes: query => query.Include(c => c.Information));

			comicBooksToDelete.ForEach(cb =>
			{
				logger.Information(
					$"ComicBookRepository.DeleteAllFromGroup: Deleting cover image for: {cb.Id} at {cb.Information.SavedCoverImageFullPath}");

				if (File.Exists(cb.Information.SavedCoverImageFullPath))
				{
					File.Delete(cb.Information.SavedCoverImageFullPath);
				}

				repository.Delete(cb.Id);
			});


			var updatedLines = dbContext.SaveChanges();
			logger.Information($"ComicBookService.DeleteAllFromGroup: deleted {updatedLines / 2} comic books");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Error delete comic books from group", Severity.Warning, e);
		}
	}

	// Misc

	public void ExtractImagesOfComicBook(int comicBookId)
	{
		CleanReadingDirectory();

		var comicBook = GetById(comicBookId);

		if (comicBook == null)
		{
			throw new ArgumentException("Comic book doesn't exist");
		}

		if (!Directory.Exists(DataPath.ComicBookReadingDirectory))
			Directory.CreateDirectory(DataPath.ComicBookReadingDirectory);

		using IArchive comicBookFile = ArchiveFactory.Open(comicBook.FileUri);

		comicBookFile
			.Entries
			.Where(entry => !entry.IsDirectory && Image.IsSupported(entry.Key!))
			.ToList()
			.ForEach(entry => entry.WriteToDirectory(DataPath.ComicBookReadingDirectory));
	}

	public string GetComicBookCoverFromDisc(int comicBookId)
	{
		var comicBook = repository.First(
			filter: cb => cb.Id == comicBookId,
			includes: query => query
				.Include(cb => cb.Information)
				.Include(cb => cb.Pages));

		if (comicBook == null)
			throw new HandledAppException("Comic book not found: " + comicBookId, Severity.Error);

		if (!File.Exists(comicBook.Information.SavedCoverImageFullPath))
		{
			logger.Warning($"Regenerating cover image for: {comicBook.Title}");
			var comicBookCover = new PageInfoHelper(comicBook.Pages).GetCover();
			new ComicBookImageHandler().SaveThumbnailToDisc(comicBook.FileUri, comicBookCover.PageFileName,
				comicBook.Information.SavedCoverImageFileName);
		}

		return comicBook.Information.SavedCoverImageFullPath;
	}


	public static void CleanReadingDirectory()
	{
		if (!Directory.Exists(DataPath.ComicBookReadingDirectory))
			return;

		foreach (var file in Directory.GetFiles(DataPath.ComicBookReadingDirectory))
		{
			File.Delete(file);
		}
	}
}
