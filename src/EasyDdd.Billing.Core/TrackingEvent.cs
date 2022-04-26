using NodaTime;

namespace EasyDdd.Billing.Core;

public record TrackingEvent(string Type, LocalDateTime Occurred)
{
	public string? Comments { get; init; }
}