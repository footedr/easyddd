using Microsoft.AspNetCore.SignalR;

namespace EasyDdd.Billing.Web.Messaging;

public class ShipmentsHub : Hub
{
	
}

public static class ShipmentsHubExtensions
{
	public static async Task PublishMessage<TMessage>(this IHubContext<ShipmentsHub> hubContext,
		string shipmentId,
		string messageType,
		TMessage message,
		CancellationToken cancellationToken = default)
	{
		await hubContext.Clients
			.All
			.SendAsync("receiveMessage", 
				shipmentId, 
				messageType, 
				message, 
				cancellationToken);
	}
}