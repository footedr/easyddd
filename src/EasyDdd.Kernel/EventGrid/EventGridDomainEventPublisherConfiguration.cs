using System;
using System.Text.Json;

namespace EasyDdd.Kernel.EventGrid
{
    public class EventGridDomainEventPublisherConfiguration
    {
        public string? Hostname { get; set; }
        public string? Key { get; set; }
        public string? Subject { get; set; }
        public string? DataVersion { get; set; }
        public Func<DomainEvent, bool>? Filter { get; set; }
        public Func<DomainEvent, string>? EventNameResolver { get; set; }
        public JsonSerializerOptions? JsonOptions { get; set; }
    }
}
