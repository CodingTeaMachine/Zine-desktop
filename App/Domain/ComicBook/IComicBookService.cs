
namespace Zine.App.Domain.ComicBook;

public interface IComicBookService
{
    public IEnumerable<ComicBook> GetAllByGroupId(int groupId);
    public ComicBook? GetById(int comicId);
    public bool AddToGroup(int groupId, int comicBookId);
    public void MoveAll(int currentGroupId, int newGroupId);
    public bool Rename(int comicBookId, string newName);
    public bool Delete(int comicId);


    public void ExtractImagesOfComicBook(int comicBookId);
}
