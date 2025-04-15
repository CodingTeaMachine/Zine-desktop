using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using SharpCompress;
using Zine.App.Database;
using Zine.App.Domain.ComicBookInformation.Enum;
using ReadingStateEnum = Zine.App.Domain.ComicBookInformation.Enum.ReadingState;

namespace Zine.App.Domain.ComicBook.DTO;

public class ComicBookSearchDto : ASearchDto<ComicBook>
{
	public string? Title { get; set; }
	public Person.Person? Drawer { get; set; }
	public Person.Person? Colorist { get; set; }
	public Person.Person? Writer { get; set; }
	public Person.Person? Editor { get; set; }
	public Publisher.Publisher? Publisher { get; set; }
	public Series.Series? Series { get; set; }
	public List<Tag.Tag> Tags { get; set; } = [];
	public StatusTag.StatusTag? StatusTag { get; set; }
	public DateRange? ReleaseDateDateRange { get; set; }
	public ReadingStateEnum? ReadingState { get; set; }

	public override IQueryable<ComicBook> ToServerQuery(IQueryable<ComicBook> query)
	{
		if (!Title.IsNullOrEmpty())
		{
			string nameQuery = Title!.ToLower();
			query = query.Where(cb => cb.Title.ToLower().Contains(nameQuery));
		}

		if (Drawer != null)
			query = query.Where(cb => cb.Information.People.Contains(Drawer));

		if (Colorist != null)
			query = query.Where(cb => cb.Information.People.Contains(Colorist));

		if (Writer != null)
			query = query.Where(cb => cb.Information.People.Contains(Writer));

		if (Editor != null)
			query = query.Where(cb => cb.Information.People.Contains(Editor));

		if (Publisher != null)
			query = query.Where(cb => cb.Information.Publishers.Contains(Publisher));

		if (Series != null)
			query = query.Where(cb => cb.Information.Series != null && cb.Information.Series.Id == Series.Id);

		if (Tags.Count != 0)
		{
			var tagIds = Tags.Select(tag => tag.Id);
			query = query.Where(cb => tagIds.All(tagId => cb.Information.Tags.Any(cbTag => cbTag.Id == tagId)));
		}

		if(StatusTag != null)
			query = query.Where(cb => cb.Information.StatusTag != null && cb.Information.StatusTag.Id == StatusTag.Id);

		if (ReleaseDateDateRange != null && ReleaseDateDateRange.Start != null && ReleaseDateDateRange.End != null)
			query = query.Where(cb =>
				cb.Information.ReleaseDate != null && cb.Information.ReleaseDate >= ReleaseDateDateRange.Start &&
				cb.Information.ReleaseDate <= ReleaseDateDateRange.End);

		return query;
	}

	public override List<ComicBook> ToClientQuery(List<ComicBook> list)
	{
		if (ReadingState != null)
		{
			var result = ReadingState switch
			{
				ReadingStateEnum.NotRead => list.Where(cb => cb.ReadPercentage == 0),
				ReadingStateEnum.Reading => list.Where(cb => cb.ReadPercentage is > 0 and < 100),
				ReadingStateEnum.Read => list.Where(cb => cb.ReadPercentage == 100),
				_ => list
			};

			list = result.ToList();
		}

		return list;
	}

	public override string ToString()
	{
		return $"Title: {Title}";
	}
}
