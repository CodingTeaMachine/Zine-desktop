using Microsoft.AspNetCore.Components.Web;

namespace Zine.App.Helpers.JsHelpers;

public interface IJsKeyEventListener
{
	public void JS_OnKeyDown(KeyboardEventArgs eventArgs);
}
