namespace Zine.App.Helpers;

public readonly struct ImageDimensions(int width, int height)
{
	public int Height { get; init; } = height;
	public int Width { get; init; } = width;
}
