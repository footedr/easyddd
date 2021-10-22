using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyDdd.Core.CreateShipment
{
	public class ShipmentRequest
	{
		[Required(ErrorMessage = "Ready window is required.")]
		public AppointmentWindowRequest ReadyWindow { get; set; } = default!;

		[Required(ErrorMessage = "Shipper is required.")]
		public LocationRequest Shipper { get; set; } = default!;

		[Required(ErrorMessage = "Consignee is required.")]
		public LocationRequest Consignee { get; set; } = default!;
		
		public List<ShipmentDetailRequest> Details { get; set; } = new List<ShipmentDetailRequest>();
	}
}