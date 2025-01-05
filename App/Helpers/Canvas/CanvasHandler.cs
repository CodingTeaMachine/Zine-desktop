using Microsoft.JSInterop;
using Zine.Components.Pages;

namespace Zine.App.Helpers.Canvas;


public class CanvasHandler : IAsyncDisposable
{

	public CanvasHandler(IJSRuntime jsRuntime, string canvasId)
	{
		_jsRuntime = jsRuntime;
		_canvasId = canvasId;

		_ = InitCanvas();
	}

	private readonly Lazy<Task<IJSObjectReference>> _moduleTask = new(() => _jsRuntime.InvokeAsync<IJSObjectReference>(
		"import", "./js/canvas/interop.js").AsTask());

	private static IJSRuntime _jsRuntime = null!;
	private readonly string _canvasId;

	private async Task InitCanvas()
	{
		var module = await _moduleTask.Value;
		await module.InvokeVoidAsync("init", _canvasId);
	}

	public async Task DrawImage(string imgSrc)
	{
		var module = await _moduleTask.Value;
		await module.InvokeVoidAsync("showComicPage", imgSrc);
	}

	public async Task ZoomIn()
	{
		var module = await _moduleTask.Value;
		await module.InvokeVoidAsync("zoom", true);
	}

	public async Task ZoomOut()
	{
		var module = await _moduleTask.Value;
		await module.InvokeVoidAsync("zoom", false);
	}

	public async Task<int> GetZoomScale()
	{
		var module = await _moduleTask.Value;
		return await module.InvokeAsync<int>("getZoomScale");
	}



	public async Task SetDotnetHelperReference(DotNetObjectReference<Reading> dotNetObjectReference)
	{
		var module = await _moduleTask.Value;
		await module.InvokeVoidAsync("setDotnetHelper", dotNetObjectReference);
	}

	public async ValueTask DisposeAsync()
	{
		GC.SuppressFinalize(this);

		if (_moduleTask.Value.IsCompleted)
		{
			var module = await _moduleTask.Value;
			await module.InvokeVoidAsync("dispose");
			await module.DisposeAsync();
		}
	}
}
