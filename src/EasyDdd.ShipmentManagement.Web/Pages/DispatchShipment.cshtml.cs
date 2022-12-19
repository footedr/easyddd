using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using EasyDdd.ShipmentManagement.Core.DispatchShipment;
using EasyDdd.ShipmentManagement.Core.Specifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace EasyDdd.ShipmentManagement.Web.Pages;

public class DispatchShipmentModel : PageModel
{
	private readonly IMediator _mediator;
	private readonly IReadModel<Shipment> _readModel;

	public DispatchShipmentModel(IMediator mediator, IReadModel<Shipment> readModel)
	{
		_mediator = mediator;
		_readModel = readModel;
	}

	public Shipment Shipment { get; private set; } = default!;

	[BindProperty]
	public DispatchRequest? DispatchRequest { get; set; }

	[FromQuery]
	public ShipmentId? ShipmentId { get; set; }

	public async Task<IActionResult> OnGet()
	{
		if (ShipmentId is null)
		{
			return RedirectToPage("/errors/404", new { msg = "Shipment was not found." });
		}

		var (shipment, actionResult) = await QueryShipment(ShipmentId);

		if (shipment is null)
		{
			return actionResult;
		}

		Shipment = shipment;

		if (Shipment.Status == ShipmentStatus.Rated && Shipment.CarrierRate is not null)
		{
			DispatchRequest = new DispatchRequest();
		}
		else
		{
			ModelState.AddModelError(string.Empty, "In order to dispatch a shipment, the shipment must be in the rated status with a valid rate.");
		}

		return actionResult;
	}

	public async Task<IActionResult> OnPost()
	{
		if (ShipmentId is null)
		{
			return RedirectToPage("/errors/404", new { msg = "Shipment was not found." });
		}

		var (shipment, actionResult) = await QueryShipment(ShipmentId);

		if (shipment is null)
		{
			return actionResult;
		}

		Shipment = shipment;

		if (!ModelState.IsValid || DispatchRequest is null)
		{
			return actionResult;
		}

		var dispatchShipmentCommand = new DispatchShipmentCommand(User, ShipmentId, DispatchRequest);
		_ = await _mediator.Send(dispatchShipmentCommand);
		
		return RedirectToPage("/ShipmentSpotlight", new { id = ShipmentId });
	}

	private async Task<(Shipment? Shipment, IActionResult ActionResult)> QueryShipment(string shipmentIdentifier)
	{
		var shipment = await _readModel.Query(User)
			.Where(new ShipmentIdSpecification(shipmentIdentifier).ToExpression())
			.SingleOrDefaultAsync()
			.ConfigureAwait(false);

		if (shipment == null)
		{
			return (default, RedirectToPage("/errors/404", new { msg = $"Shipment #{shipmentIdentifier} was not found." }));
		}

		return (shipment, Page());
	}
}