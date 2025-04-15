using Zine.App.Common.Service.DTO;
using Zine.App.Database;
using Zine.App.Logger;

namespace Zine.App.Domain.StatusTag;

public class StatusTagService(GenericRepository<StatusTag> repository, ILoggerService logger) : IStatusTagService
{
	public StatusTag Create(object obj)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<StatusTag> Search(object searchParams)
	{
		var searchDto = (GenericSearchDto)searchParams;

		if(searchDto.SearchTerm == null)
			return repository.List();

		var nameToLower = searchDto.SearchTerm.ToLower();

		return repository.List(filter: st => st.Name.ToLower().Contains(nameToLower));
	}

	public void Delete(StatusTag value)
	{
		throw new NotImplementedException();
	}
}
