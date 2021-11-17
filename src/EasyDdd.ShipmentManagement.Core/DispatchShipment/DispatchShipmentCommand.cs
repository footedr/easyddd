using System.Security.Claims;
using EasyDdd.Kernel;

namespace EasyDdd.ShipmentManagement.Core.DispatchShipment;

public record DispatchShipmentCommand(ClaimsPrincipal User, string ShipmentIdentifier, DispatchRequest DispatchRequest)
	: Command<Dispatch>
{
}