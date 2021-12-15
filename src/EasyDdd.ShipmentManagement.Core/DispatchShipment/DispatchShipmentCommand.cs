using System.Security.Claims;
using EasyDdd.Kernel;

namespace EasyDdd.ShipmentManagement.Core.DispatchShipment;

public record DispatchShipmentCommand(ClaimsPrincipal User, ShipmentId ShipmentId, DispatchRequest DispatchRequest)
	: Command<Dispatch>
{
}