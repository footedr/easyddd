using System.Security.Claims;
using EasyDdd.Kernel;

namespace EasyDdd.Core.DispatchShipment;

public record DispatchShipmentCommand(ClaimsPrincipal User, string ShipmentIdentifier, DispatchRequest DispatchRequest)
	: Command<Dispatch>
{
}