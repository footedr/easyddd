using System.Security.Claims;
using MediatR;

namespace EasyDdd.Core.CreateShipment
{
	public record CreateShipmentCommand(ClaimsPrincipal User, ShipmentRequest Shipment)
		: IRequest<Shipment>
	{
	}
}