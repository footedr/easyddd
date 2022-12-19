using EasyDdd.Billing.Core;
using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.AspNetCore.SignalR;

namespace EasyDdd.Billing.Web.Messaging;

public class ShipmentCreatedHandler : ExternalEventHandler<ShipmentCreated>
{
	private readonly IHubContext<MessageHub> _hubContext;

	public ShipmentCreatedHandler(IHubContext<MessageHub> hubContext)
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
	private readonly IHubContext<MessageHub> _hubContext;

	public ShipmentRatedHandler(IHubContext<MessageHub> hubContext)
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
	private readonly IHubContext<MessageHub> _hubContext;

	public ShipmentStatusUpdatedHandler(IHubContext<MessageHub> hubContext)
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
	private readonly IHubContext<MessageHub> _hubContext;

	public ShipmentDispatchedHandler(IHubContext<MessageHub> hubContext)
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
	private readonly IHubContext<MessageHub> _hubContext;

	public ShipmentDeliveredHandler(IHubContext<MessageHub> hubContext)
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
	private readonly IHubContext<MessageHub> _hubContext;

	public TrackingEventAddedHandler(IHubContext<MessageHub> hubContext)
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

public class StatementLineAddedHandler : DomainEventHandler<StatementLineAdded>
{
	private readonly IHubContext<MessageHub> _hubContext;
	private readonly ILogger<StatementLineAddedHandler> _logger;

	public StatementLineAddedHandler(IHubContext<MessageHub> hubContext,
		ILogger<StatementLineAddedHandler> logger)
	{
		_hubContext = hubContext;
		_logger = logger;
	}

	public override async Task Handle(StatementLineAdded @event, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Received {EventType} event for statement #{StatementId}.", nameof(StatementLineAdded),
			@event.StatementIdentifier.Value);

		await _hubContext.PublishMessage(@event.StatementIdentifier,
			"statementLineAdded",
			@event,
			cancellationToken);

		_logger.LogInformation("SignalR message sent for {EventType} event for statement #{StatementId}.",
			nameof(StatementLineAdded), @event.StatementIdentifier.Value);
	}
}

public class StatementCreatedHandler : DomainEventHandler<StatementCreated>
{
	private readonly IHubContext<MessageHub> _hubContext;
	private readonly ILogger<StatementCreatedHandler> _logger;

	public StatementCreatedHandler(IHubContext<MessageHub> hubContext,
		ILogger<StatementCreatedHandler> logger)
	{
		_hubContext = hubContext;
		_logger = logger;
	}

	public override async Task Handle(StatementCreated @event, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Received {EventType} event for statement #{StatementId}.", nameof(StatementCreated),
			@event.Statement.Identifier.Value);

		await _hubContext.PublishMessage(@event.Statement.Identifier,
			"statementCreated",
			@event,
			cancellationToken);

		_logger.LogInformation("SignalR message sent for {EventType} event for statement #{StatementId}.",
			nameof(StatementCreated), @event.Statement.Identifier.Value);
	}
}