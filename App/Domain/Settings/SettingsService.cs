using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Zine.App.Enums;
using Zine.App.Exceptions;
using Zine.App.Logger;

namespace Zine.App.Domain.Settings;

public class SettingsService(ILoggerService logger, IConfiguration configuration) : ISettingsService
{
	public Settings Get()
	{
		return SettingsJsonConverter.Read(
			configuration.GetConnectionString(ConfigKeys.SettingsLocation)!
			);
	}
	
	public void UpdateMany(Settings settings)
	{
		try
		{
			SettingsJsonConverter.Write(
				configuration.GetConnectionString(ConfigKeys.SettingsLocation)!,
				settings
			);

			logger.Information("Saved settings updates");
		}
		catch (DbUpdateException e)
		{
			throw new HandledAppException("Could not save settings", Severity.Error, e);
		}
	}
}
