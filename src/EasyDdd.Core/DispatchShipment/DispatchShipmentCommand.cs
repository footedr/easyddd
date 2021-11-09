using System.Security.Claims;
using MediatR;

namespace EasyDdd.Core.DispatchShipment;

public record DispatchShipmentCommand(ClaimsPrincipal User, string ShipmentIdentifier, DispatchRequest DispatchRequest)
	: IRequest<Dispatch>
{
}