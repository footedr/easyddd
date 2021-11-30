using System;
using NodaTime;

namespace EasyDdd.ShipmentManagement.Core.Tracking;

public class TrackingEventRequest
{
	public string? TypeCode { get; set; }
	public LocalDate? DeliveredDate { get; set; }
	public TimeSpan? DeliveredTime { get; set; }
	public string? Comments { get; set; }
}