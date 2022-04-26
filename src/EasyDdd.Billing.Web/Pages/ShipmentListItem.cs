using EasyDdd.Billing.Core;
using NodaTime;

namespace EasyDdd.Billing.Web.Pages;

public record ShipmentListItem(string Identifier, string Status)
{
	public decimal? TotalCost { get; init; }
	public string? CarrierName { get; init; }
	public string? DispatchNumber { get; init; }
	public string? PickupNumber { get; init; }
	public LocalDateTime? DispatchDateTime { get; init; }
	public TrackingEvent? TrackingEvent { get; init; }
}