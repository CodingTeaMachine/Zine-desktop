namespace Zine.App.Domain.Settings;

public class Settings
{
	public Setting<int> HomaPageRecommendationCount = new() { Value = 10, DefaultValue = 10 };
	public Setting<int> HomePageRecentlyReadCountCount = new() { Value = 20, DefaultValue = 20 };
}
