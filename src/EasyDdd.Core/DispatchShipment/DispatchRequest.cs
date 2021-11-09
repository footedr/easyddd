using System;
using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace EasyDdd.Core.DispatchShipment;

public class DispatchRequest
{
	[Required(ErrorMessage = "Carrier pickup number is required.")]
	public string? PickupNumber { get; set; }
	public string? ReferenceNumber { get; set; }
	public string? PickupNote { get; set; }
	[Required(ErrorMessage = "Dispatch date is required.")]
	[DataType(DataType.Date)]
	public LocalDate? DispatchDate { get; set; }
	[Required(ErrorMessage = "Dispatch time is required.")]
	[DataType(DataType.Time)]
	public TimeSpan? DispatchTime { get; set; }
}