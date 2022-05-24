using System.Text.Json;

namespace EasyDdd.Kernel.EventHubs;

public class KafkaPublisherConfiguration
{
	public KafkaPublisherConfiguration(string endpoint, JsonSerializerOptions jsonSerializerOptions)
	{
		Endpoint = endpoint;
		JsonSerializerOptions = jsonSerializerOptions;
	}

	public string Endpoint { get; }
	public JsonSerializerOptions JsonSerializerOptions { get; }
}