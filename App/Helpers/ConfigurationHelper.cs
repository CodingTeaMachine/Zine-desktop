namespace Zine.App.Helpers;

public static class ConfigurationHelper
{
	public static IConfiguration Config = null!;
	public static void Initialize(IConfiguration configuration)
	{
		Config = configuration;
	}
}
