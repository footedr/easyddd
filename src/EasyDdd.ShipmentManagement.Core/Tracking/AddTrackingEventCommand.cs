using System.Security.Claims;
using EasyDdd.Kernel;

namespace EasyDdd.ShipmentManagement.Core.Tracking;

public record AddTrackingEventCommand(ClaimsPrincipal User, ShipmentId ShipmentId, TrackingEventRequest TrackingEventRequest)
	: Command<TrackingEvent>
{
}