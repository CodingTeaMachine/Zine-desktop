using MudBlazor;

namespace Zine.App.Domain.StatusTag;

public static class BaseStatusTags
{
	public static StatusTag[] StatusTags =>
	[
		new() {Id = 1, Name = "Finished", Color = Color.Success},
		new() {Id = 2, Name = "Reading", Color = Color.Info},
		new() {Id = 3, Name = "Archived", Color = Color.Primary},
		new() {Id = 4, Name = "Want to read", Color = Color.Secondary},
		new() {Id = 5, Name = "Retired", Color = Color.Error},
	];
}
