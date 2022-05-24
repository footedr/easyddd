using System.Text.Json;

namespace EasyDdd.Kernel.EventHubs;

public class KafkaConsumerConfiguration
{
	public KafkaConsumerConfiguration(string topicName, string consumerGroup, string endpoint, JsonSerializerOptions jsonSerializerOptions)
	{
		TopicName = topicName;
		ConsumerGroup = consumerGroup;
		Endpoint = endpoint;
		JsonSerializerOptions = jsonSerializerOptions;
	}

	public string TopicName { get; }
	public string ConsumerGroup { get; }
	public string Endpoint { get; }
	public JsonSerializerOptions JsonSerializerOptions { get; }
}