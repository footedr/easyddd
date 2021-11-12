using System.Security.Claims;
using EasyDdd.Kernel;

namespace EasyDdd.Core.Tracking;

public record AddTrackingEventCommand(ClaimsPrincipal User, string ShipmentIdentifier, TrackingEventRequest TrackingEventRequest)
	: Command<TrackingEvent>
{
}