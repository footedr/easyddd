namespace EasyDdd.ShipmentManagement.Core.CreateShipment
{
	public class ShipmentDetailRequest
	{
		public string? Description { get; set; } = default!;
		public string Class { get; set; } = default!;
		public int? Weight { get; set; }
		public int? HandlingUnitCount { get; set; }
		public string PackagingType { get; set; } = default!;
		public bool IsHazardous { get; set; }
	}
}