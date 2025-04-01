using Zine.App.Common.Service;

namespace Zine.App.Domain.Tag;

public interface ITagService : IDropdownService<Tag>
{
	public IEnumerable<Tag> List();
}
