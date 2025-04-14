namespace Zine.App.Common;

public class Debouncer : IDisposable
{
	private CancellationTokenSource _cancellationTokenSource = new();

	public void Debounce(int milliseconds, Action action)
	{
		_cancellationTokenSource.Cancel();
		_cancellationTokenSource = new();

		var token = _cancellationTokenSource.Token;

		Task.Run(async () =>
		{
			try
			{
				await Task.Delay(milliseconds, token);
				if (!token.IsCancellationRequested)
				{
					action();
				}
			}
			catch (TaskCanceledException)
			{
				// ignore
			}
		}, token);

	}


	public void DebounceAsync(int milliseconds, Action<Task> action)
	{
		_cancellationTokenSource.Cancel();
		_cancellationTokenSource = new();
		
		Task.Delay(milliseconds, _cancellationTokenSource.Token).ContinueWith(action, _cancellationTokenSource.Token);
	}

	public void Dispose()
	{
		_cancellationTokenSource.Dispose();
	}
}
