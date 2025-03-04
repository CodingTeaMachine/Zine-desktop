
namespace Zine.App.Domain.ComicBook;

public interface IComicBookService
{
    public void ExtractImagesOfComicBook(int comicBookId);
    public IEnumerable<ComicBook> SearchByTitle(string searchTerm);

    public ComicBook Create(string title, string fileUri, int groupId);
    public ComicBook? GetById(int comicId);
    public string GetComicBookCoverFromDisc(int comicBookId);
    public ComicBook? GetForReadingView(int comicId);
    public void AddToGroup(int groupId, int comicBookId);
    public void MoveAll(int currentGroupId, int newGroupId);
    public void Rename(int comicBookId, string newTitle);
    public void Delete(int comicId);
    public void DeleteAllFromGroup(int groupId);
}
