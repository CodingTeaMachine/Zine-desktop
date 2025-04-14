
using Zine.App.Domain.ComicBook.DTO;

namespace Zine.App.Domain.ComicBook;

public interface IComicBookService
{
    //Create
    public ComicBook Create(string title, string fileUri, int groupId);

    //Read
    public ComicBook? GetById(int comicId);
    public IEnumerable<ComicBook> GetCurrentlyReadComicBooks(int count);
    public IEnumerable<ComicBook> GetRecommendations(int recommendationCount);
    public string GetComicBookCoverFromDisc(int comicBookId);
    public ComicBook GetForReadingView(int comicId);
    public ComicBook GetForInformationDrawer(int comicId);
    public IEnumerable<ComicBook> Search(ComicBookSearchDto search);

    //Update
    public void AddToGroup(int groupId, int comicBookId);
    public void MoveAll(int currentGroupId, int newGroupId);
    public void Rename(int comicBookId, string newTitle);
    public ComicBook Update(ComicBook comicBook);

    //Delete
    public void Delete(int comicId);
    public void DeleteAllFromGroup(int groupId);

    //Misc
    public void ExtractImagesOfComicBook(int comicBookId);
}
