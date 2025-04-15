using Microsoft.AspNetCore.SignalR;

namespace Zine.App.Hubs;

public class CloseComicBookHub : Hub
{
	public async Task SendCloseComicBookSignal()
	{
		await Clients.All.SendAsync("ReceiveCloseComicBookSignal");
	}
}
