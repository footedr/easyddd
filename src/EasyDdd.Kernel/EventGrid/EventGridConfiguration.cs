using System.Text.Json;

namespace EasyDdd.Kernel.EventGrid
{
    public class EventGridConfiguration
    {
        public EventGridConfiguration(string apiKey, JsonSerializerOptions? jsonOptions)
        {
            ApiKey = apiKey;
            JsonOptions = jsonOptions;
        }

        public string ApiKey { get; }
        public JsonSerializerOptions? JsonOptions { get; }
    }
}
