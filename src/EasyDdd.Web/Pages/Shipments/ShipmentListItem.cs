using EasyDdd.Core;
using EasyDdd.Core.CreateShipment;

namespace EasyDdd.Web.Pages.Shipments
{
	public record ShipmentListItem(string Identifier, string Status, AppointmentWindowRequest ReadyWindow, LocationRequest Shipper, LocationRequest Consignee);
}