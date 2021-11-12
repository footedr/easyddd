using System.Security.Claims;
using EasyDdd.Kernel;

namespace EasyDdd.Core.CreateShipment
{
	public record CreateShipmentCommand(ClaimsPrincipal User, ShipmentRequest Shipment)
		: Command<Shipment>
	{
	}
}