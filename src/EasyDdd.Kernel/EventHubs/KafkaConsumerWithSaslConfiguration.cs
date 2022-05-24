using System.Text.Json;

namespace EasyDdd.Kernel.EventHubs;

public class KafkaConsumerWithSaslConfiguration : KafkaConsumerConfiguration
{
	public KafkaConsumerWithSaslConfiguration(string topicName, string consumerGroup, string endpoint, string connectionString, JsonSerializerOptions jsonSerializerOptions)
		: base(topicName, consumerGroup, endpoint, jsonSerializerOptions)
	{
		ConnectionString = connectionString;
	}
	public string ConnectionString { get; }
}