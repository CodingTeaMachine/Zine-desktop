namespace Zine.App.Domain.Settings;

public class Setting<T>
{
	public required T Value { get; set; }
	public required T DefaultValue { get; set; }
}
