using System.ComponentModel.DataAnnotations;

namespace EasyDdd.Web.Pages.Shipments
{
	public class ShipmentRequest
	{
		[Required(ErrorMessage = "Ready window is required.")]
		public AppointmentWindowRequest ReadyWindow { get; set; } = default!;

		[Required(ErrorMessage = "Shipper is required.")]
		public LocationRequest Shipper { get; set; } = default!;

		[Required(ErrorMessage = "Consignee is required.")]
		public LocationRequest Consignee { get; set; } = default!;
	}
}