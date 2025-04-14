namespace Zine.App.Domain.Settings;

public interface ISettingsService
{
	public Settings Get();
	
	public void UpdateMany(Settings settings);
}
