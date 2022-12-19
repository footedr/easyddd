using Microsoft.AspNetCore.SignalR;

namespace EasyDdd.Billing.Web.Messaging;

public class MessageHub : Hub
{
	
}

public static class MessageHubExtensions
{
	public static async Task PublishMessage<TMessage>(this IHubContext<MessageHub> hubContext,
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