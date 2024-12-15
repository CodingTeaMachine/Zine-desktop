namespace Zine.App.Helpers.JsHelpers;

public interface IJsScrollEventListener
{
	public void ElementScrolled(ScrollEvent direction) { }

	public Task ElementScrolledAsync(ScrollEvent direction)
	{
		return Task.CompletedTask;
	}
}
