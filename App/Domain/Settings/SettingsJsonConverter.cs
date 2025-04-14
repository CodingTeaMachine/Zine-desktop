using System.Text.Json;

namespace Zine.App.Domain.Settings;

public static class SettingsJsonConverter
{

	private static readonly JsonSerializerOptions JsonOptions = new() {WriteIndented = true, IncludeFields = true};

	public static void WriteDefaultIfNeeded(string destination)
	{

		Console.WriteLine("Writing default settings...");

		//If the file already exists read the existing settings
		//If a given key is now written to disc, the default initializer will run, so we write the new values back
		var defaultSettings = File.Exists(destination)
			? Read(destination)
			: new Settings();

		string defaultSettingsContent = JsonSerializer.Serialize(defaultSettings, JsonOptions);
		
		CreateDirectoryIfNeeded(destination);
		
		File.WriteAllText(destination, defaultSettingsContent);
	}
	
	public static void Write(string destination, Settings settings)
	{
		Console.WriteLine("Writing settings...");
		string defaultSettingsContent = JsonSerializer.Serialize(settings, JsonOptions);
		
		CreateDirectoryIfNeeded(destination);
		
		File.WriteAllText(destination, defaultSettingsContent);
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="destination"></param>
	/// <returns></returns>
	/// <exception cref="FileNotFoundException"></exception>
	/// <exception cref="NullReferenceException"></exception>
	public static Settings Read(string destination)
	{
		if(!File.Exists(destination))
			throw new FileNotFoundException("Settings file does not exist", destination);

		Console.WriteLine("Reading settings...");
		var settingsContent = File.ReadAllText(destination);
		var settings = JsonSerializer.Deserialize<Settings>(settingsContent, JsonOptions);

		if(settings == null)
			throw new NullReferenceException("Settings could not be read.");

		return settings;
	}

	private static void CreateDirectoryIfNeeded(string path)
	{
		var destinationDirectory = Path.GetDirectoryName(path)!;
		if(!Directory.Exists(destinationDirectory))
			Directory.CreateDirectory(destinationDirectory);
	}
}
