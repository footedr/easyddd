using System;
using NodaTime;

namespace EasyDdd.ShipmentManagement.Core.Tracking;

public class TrackingEventRequest
{
	public string? TypeCode { get; set; }
	public LocalDate? OccurredDate { get; set; }
	public TimeSpan? OccurredTime { get; set; }
	public string? Comments { get; set; }
}