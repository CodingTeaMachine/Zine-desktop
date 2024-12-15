using Microsoft.JSInterop;

namespace Zine.App.Helpers.Canvas;


public class CanvasHandler(IJSRuntime jsRuntime, string canvasId) : IAsyncDisposable
{
	private readonly Lazy<Task<IJSObjectReference>> _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
		"import", "./js/canvasApiJsInterop.js").AsTask());
	
	private bool _isCanvasInitialized = false;

	public async Task DrawImage(string imgSrc)
	{
		if (!_isCanvasInitialized)
		{
			await InitCanvas();
		}
		
		var module = await _moduleTask.Value;
		await module.InvokeVoidAsync("drawImage", imgSrc);
	}

	public async Task ZoomIn()
	{
		var module = await _moduleTask.Value;
		await module.InvokeVoidAsync("zoomInImage");
	}

	public async Task ZoomOut()
	{
		var module = await _moduleTask.Value;
		await module.InvokeVoidAsync("zoomOutImage");
	}

	public async ValueTask<int> GetZoomScale()
	{
		var module = await _moduleTask.Value;
		var scaleAsDecimal = await module.InvokeAsync<float>("getZoomScale");

		Console.WriteLine((int)(scaleAsDecimal * 100));

		return (int)(scaleAsDecimal * 100);
	}

	public async ValueTask DisposeAsync()
	{
		if (_moduleTask.Value.IsCompleted)
		{
			var module = await _moduleTask.Value;
			await module.InvokeVoidAsync("dispose");
			await module.DisposeAsync();
		}
	}

	private async Task InitCanvas()
	{
		var module = await _moduleTask.Value;
		await module.InvokeVoidAsync("initCanvas", canvasId);
		_isCanvasInitialized = true;
	}
}
