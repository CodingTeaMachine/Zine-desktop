using Microsoft.AspNetCore.Components.Web;

namespace Zine.App.Helpers;

public interface IJsKeyEventListener
{
	public void JsOnKeyDown(KeyboardEventArgs eventArgs);
}
