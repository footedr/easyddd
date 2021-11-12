﻿using System.Security.Claims;
using EasyDdd.Kernel;

namespace EasyDdd.Core.RateShipment
{
	public record RateShipmentCommand(ClaimsPrincipal User, string ShipmentIdentifier, RateRequest RateRequest)
		: Command<Rate>
	{
	}
}