using Microsoft.IdentityModel.Tokens;
using Zine.App.Database;

namespace Zine.App.Domain.Group.DTO;

public class GroupSearchDto : ASearchDto<Group>
{
	public string? Name { get; set; }

	public override IQueryable<Group> ToServerQuery(IQueryable<Group> query)
	{
		if (!Name.IsNullOrEmpty())
		{
			string queryName = Name!.ToLower();
			query = query.Where(g => g.Name.ToLower().Contains(queryName));
		}

		return query;
	}

	public override string ToString()
	{
		return $"Name: {Name}";
	}
}
