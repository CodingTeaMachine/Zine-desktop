namespace Zine.App.Helpers.JsHelpers;

public interface IJsScrollEventListener
{
	public void JS_ElementScrolled(ScrollEvent direction) { }

	public Task JS_ElementScrolledAsync(ScrollEvent direction)
	{
		return Task.CompletedTask;
	}
}
