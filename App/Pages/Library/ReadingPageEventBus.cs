namespace Zine.App.Pages.Library;

public class ReadingPageEventBus
{
	private readonly Dictionary<EventName, List<Delegate>> _eventHandlers = new();

	public Action Subscribe<T>(EventName eventName, Action<T> handler)
	{
		if(!_eventHandlers.ContainsKey(eventName))
			_eventHandlers[eventName] = new List<Delegate>();

		_eventHandlers[eventName].Add(handler);

		return () => Unsubscribe(eventName, handler);
	}

	private void Unsubscribe<T>(EventName eventName, Action<T> handler)
	{
		if (!_eventHandlers.TryGetValue(eventName, out var eventHandler))
			return;

		eventHandler.Remove(handler);

		if(eventHandler.Count == 0)
			_eventHandlers.Remove(eventName);
	}

	public void Publish<T>(EventName eventName, T data)
	{
		if (!_eventHandlers.TryGetValue(eventName, out var eventHandlers))
			return;


		foreach (var handler in eventHandlers)
		{
			if(handler is Action<T> typedHandler)
				typedHandler(data);
		}
	}
}
