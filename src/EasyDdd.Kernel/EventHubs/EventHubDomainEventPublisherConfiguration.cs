using System.Text.Json;

namespace EasyDdd.Kernel.EventHubs;

public class EventHubDomainEventPublisherConfiguration
{
	public string Endpoint { get; set; } = default!;
	public string ConnectionString { get; set; } = default!;
	public JsonSerializerOptions JsonSerializerOptions { get; set; } = default!;
}