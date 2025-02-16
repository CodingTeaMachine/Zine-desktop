using Microsoft.EntityFrameworkCore;
using MudBlazor;
using SharpCompress;
using SharpCompress.Archives;
using Zine.App.Database;
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
	ILoggerService logger
) : IComicBookService
{
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

	public IEnumerable<ComicBook> GetAllByGroupId(int groupId)
	{
		return repository
			.List(
				filter: cb => cb.GroupId == groupId,
				includes: query => query.Include(c => c.Information))
			.Select(cb =>
			{
				switch (cb.Information.FileMovedOrDeleted)
				{
					case true:
						logger.Warning($"{cb.FileUri} moved/deleted");
						break;
					//Check if the cover image exists, and if not, regenerate it.
					case false when !File.Exists(Path.Join(DataPath.ComicBookCoverDirectory,
						cb.Information.SavedCoverImageFileName)):
						//TODO: Move to comic book information service or cover image handler
						logger.Warning($"Regenerating cover image for: {cb.Title}");
						var coverImage = cb.Pages.First(page => page.PageType == PageType.Cover);
						new ComicBookInformationFactory().SaveThumbnailToDisc(coverImage.PageFileName, cb.FileUri,
							cb.Id.ToString());
						break;
				}


				return cb;
			});
	}

	public ComicBook? GetById(int comicId)
	{
		return repository.GetById(comicId);
	}

	public ComicBook? GetWithPages(int comicId)
	{
		return repository.First(
			filter: c => c.Id == comicId,
			includes: query => query.Include(c => c.Pages));
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="searchTerm"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public IEnumerable<ComicBook> SearchByTitle(string searchTerm)
	{
		searchTerm = searchTerm.ToLower();
		logger.Information($"ComicBookService.SearchByTitle: Searching for comic book by name: \"{searchTerm}\"");

		try
		{
			var comicBooks = repository.List(f => f.Title.ToLower().Contains(searchTerm)).ToArray();
			logger.Information(
				$"ComicBookService.SearchByTitle: Found {comicBooks.Length} comic books for term: \"{searchTerm}\"");
			return comicBooks;
		}
		catch (Exception e)
		{
			throw new HandledAppException("Error searching comic books", Severity.Error, e);
		}
	}



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

	public void Delete(int comicId)
	{
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
			var comicBooksToDelete = repository.List(cb => cb.GroupId == groupId);

			comicBooksToDelete.ForEach(cb =>
			{
				logger.Information(
					$"ComicBookRepository.DeleteAllFromGroup: Deleting cover image for: {cb.Id} at {cb.Information.SavedCoverImageFullPath}");
				File.Delete(cb.Information.SavedCoverImageFullPath);
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


	public void ExtractImagesOfComicBook(int comicBookId)
	{
		CleanReadingDirectory();

		Console.WriteLine("Extracting images");

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
			.ForEach(entry => entry.WriteToDirectory(DataPath.ComicBookReadingDirectory));
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
