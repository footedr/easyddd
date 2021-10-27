using System.Security.Claims;
using MediatR;

namespace EasyDdd.Core.RateShipment
{
	public record RateShipmentCommand(ClaimsPrincipal User, string ShipmentIdentifier, RateRequest RateRequest)
		: IRequest<Rate>
	{
	}
}