using EasyDdd.ShipmentManagement.Core;

namespace EasyDdd.ShipmentManagement.Web.Pages;

public record ShipmentListItem(string Identifier, string Status, AppointmentWindowRequest ReadyWindow, LocationRequest Shipper, LocationRequest Consignee);