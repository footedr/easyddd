using EasyDdd.Billing.Core.Specifications;
using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;

namespace EasyDdd.Billing.Core.EventHandlers
{
	public class ShipmentDeliveredHandler : ExternalEventHandler<ShipmentDelivered>
	{
		private readonly IOptions<BillingOptions> _billingOptions;
		private readonly ILogger<ShipmentDeliveredHandler> _logger;
		private readonly IRepository<Shipment> _shipmentRepository;
		private readonly StatementService _statementService;

		public ShipmentDeliveredHandler(ILogger<ShipmentDeliveredHandler> logger,
			IRepository<Shipment> shipmentRepository,
			StatementService statementService,
			IOptions<BillingOptions> billingOptions)
		{
			_logger = logger;
			_shipmentRepository = shipmentRepository;
			_statementService = statementService;
			_billingOptions = billingOptions;
		}

		public override async Task Handle(ShipmentDelivered @event, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received {EventType} event for shipment# {ShipmentIdentifier}.", nameof(ShipmentDelivered), @event.ShipmentIdentifier);

			var shipment = (await _shipmentRepository.FindAsync(new ShipmentByIdSpecification(@event.ShipmentIdentifier))
					.ConfigureAwait(false))
				.SingleOrDefault();

			if (shipment == null)
			{
				_logger.LogError("Attempting to deliver shipment with id: {ShipmentId}, but shipment was not found.", @event.ShipmentIdentifier);
				return;
			}

			shipment.UpdateDeliveryDate(@event.DeliveredAt);

			await _shipmentRepository.SaveAsync(shipment);

			_logger.LogInformation("Shipment with id: {ShipmentId} was delivered successfully.", @event.ShipmentIdentifier);

			var transactionFeeLine = CreateTransactionFeeLine(
				shipment,
				_billingOptions.Value.TransactionFee,
				LocalDate.FromDateTime(@event.Occurred.DateTime));

			var statement = await _statementService.AddLineToOpenStatement(_billingOptions.Value.CustomerCode, _billingOptions.Value.BillToAccount, _billingOptions.Value.BillToLocation, transactionFeeLine);

			_logger.LogInformation("Transaction fee for shipment {ShipmentIdentifier} added to statement {StatementIdentifier}", transactionFeeLine.TmsNumber, statement.Identifier);
		}

		private StatementLine CreateTransactionFeeLine(Shipment shipment, decimal feeAmount, LocalDate transactionDate)
		{
			var line = new StatementLine(
				shipment.Identifier,
				$"{shipment.Shipper.City}, {shipment.Shipper.StateAbbreviation} to {shipment.Consignee.City}, {shipment.Consignee.StateAbbreviation}",
				1,
				feeAmount,
				transactionDate)
			{
				HandlingUnits = shipment.Details.Sum(d => d.HandlingUnitCount),
				Class = string.Join(", ", shipment.Details.Select(d => d.Class)),
				Weight = shipment.Details.Sum(d => d.Weight)
			};

			return line;
		}
	}
}

namespace EasyDdd.ShipmentManagement.Core
{
	public record ShipmentDelivered(string ShipmentIdentifier, LocalDateTime DeliveredAt, DateTimeOffset Occurred)
		: ExternalEvent
	{
	}
}