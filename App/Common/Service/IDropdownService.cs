namespace Zine.App.Common.Service;

public interface IDropdownService<T>
{
	public T Create(object obj);

	public IEnumerable<T> Search(object searchParams);

	public void Delete(T value);
}
