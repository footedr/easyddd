using System.Text.Json;

namespace EasyDdd.Kernel.EventHubs;

public class EventHubDomainEventConsumerConfiguration
{
	public string TopicName { get; set; } = default!;
	public string ConsumerGroup { get; set; } = default!;
	public string ConnectionString { get; set; } = default!;
	public string Endpoint { get; set; } = default!;
	public JsonSerializerOptions JsonSerializerOptions { get; set; } = default!;
}