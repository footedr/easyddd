using System.Text.Json;

namespace EasyDdd.Kernel.EventHubs;

public class KafkaPublisherWithSaslConfiguration : KafkaPublisherConfiguration
{
	public KafkaPublisherWithSaslConfiguration(string endpoint, string connectionString, JsonSerializerOptions jsonSerializerOptions) : base(endpoint, jsonSerializerOptions)
	{
		ConnectionString = connectionString;
	}
	public string ConnectionString { get; }
}