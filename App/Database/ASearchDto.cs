using System.Collections;
using MudBlazor;
using Zine.App.Domain.ComicBook;

namespace Zine.App.Database;

public abstract class ASearchDto<T>
{
	public abstract IQueryable<T> ToServerQuery(IQueryable<T> query);

	public virtual List<T> ToClientQuery(List<T> list)
	{
		return list;
	}

	public bool AllKeysAreNullOrEmpty(bool emptyListCheck = true)
	{
		return GetType()
			.GetProperties()
			.All(k =>
			{
				var value = k.GetValue(this);

				if (emptyListCheck && value is IEnumerable listValue && !listValue.OfType<object>().Any())
				{
					return true;
				}
				
				switch (value)
				{
					case null:
					case string valueStr when string.IsNullOrWhiteSpace(valueStr):
					case DateRange { Start: null, End: null }:
						return true;
					default:
						return false;
				}
			});
	}
}
