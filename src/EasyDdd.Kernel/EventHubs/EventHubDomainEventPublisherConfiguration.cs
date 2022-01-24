namespace EasyDdd.Kernel.EventHubs;

public class EventHubDomainEventPublisherConfiguration
{
	public string Endpoint { get; set; } = default!;
	public string ConnectionString { get; set; } = default!;
}