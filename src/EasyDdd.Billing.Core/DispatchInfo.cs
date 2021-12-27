using NodaTime;

namespace EasyDdd.Billing.Core;

public record DispatchInfo(string DispatchNumber,
	string PickupNumber,
	LocalDateTime DispatchDateTime)
{
	public string? PickupNote { get; init; }
	public string? ReferenceNumber { get; init; }
}