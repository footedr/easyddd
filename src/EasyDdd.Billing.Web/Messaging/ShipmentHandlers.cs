using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.AspNetCore.SignalR;

namespace EasyDdd.Billing.Web.Messaging;

public class ShipmentCreatedHandler : ExternalEventHandler<ShipmentCreated>
{
	private readonly IHubContext<ShipmentsHub> _hubContext;

	public ShipmentCreatedHandler(IHubContext<ShipmentsHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public override async Task Handle(ShipmentCreated notification, CancellationToken cancellationToken)
	{
		await _hubContext.PublishMessage(notification.Shipment.Identifier,
			"shipmentCreated",
			notification,
			cancellationToken);
	}
}

public class ShipmentRatedHandler : ExternalEventHandler<ShipmentRated>
{
	private readonly IHubContext<ShipmentsHub> _hubContext;

	public ShipmentRatedHandler(IHubContext<ShipmentsHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public override async Task Handle(ShipmentRated notification, CancellationToken cancellationToken)
	{
		await _hubContext.PublishMessage(notification.ShipmentIdentifier,
			"shipmentRated",
			notification,
			cancellationToken);
	}
}

public class ShipmentStatusUpdatedHandler : ExternalEventHandler<ShipmentStatusUpdated>
{
	private readonly IHubContext<ShipmentsHub> _hubContext;

	public ShipmentStatusUpdatedHandler(IHubContext<ShipmentsHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public override async Task Handle(ShipmentStatusUpdated notification, CancellationToken cancellationToken)
	{
		await _hubContext.PublishMessage(notification.ShipmentIdentifier,
			"shipmentStatusUpdated",
			notification,
			cancellationToken);
	}
}

public class ShipmentDispatchedHandler : ExternalEventHandler<ShipmentDispatched>
{
	private readonly IHubContext<ShipmentsHub> _hubContext;

	public ShipmentDispatchedHandler(IHubContext<ShipmentsHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public override async Task Handle(ShipmentDispatched notification, CancellationToken cancellationToken)
	{
		await _hubContext.PublishMessage(notification.ShipmentIdentifier,
			"shipmentDispatched",
			notification,
			cancellationToken);
	}
}

public class ShipmentDeliveredHandler : ExternalEventHandler<ShipmentDelivered>
{
	private readonly IHubContext<ShipmentsHub> _hubContext;

	public ShipmentDeliveredHandler(IHubContext<ShipmentsHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public override async Task Handle(ShipmentDelivered notification, CancellationToken cancellationToken)
	{
		await _hubContext.PublishMessage(notification.ShipmentIdentifier,
			"shipmentDelivered",
			notification,
			cancellationToken);
	}
}

public class TrackingEventAddedHandler : ExternalEventHandler<TrackingEventAdded>
{
	private readonly IHubContext<ShipmentsHub> _hubContext;

	public TrackingEventAddedHandler(IHubContext<ShipmentsHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public override async Task Handle(TrackingEventAdded notification, CancellationToken cancellationToken)
	{
		await _hubContext.PublishMessage(notification.ShipmentIdentifier,
			"trackingEventAdded",
			notification,
			cancellationToken);
	}
}