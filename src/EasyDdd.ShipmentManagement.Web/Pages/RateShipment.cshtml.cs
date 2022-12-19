using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using EasyDdd.ShipmentManagement.Core.RateShipment;
using EasyDdd.ShipmentManagement.Core.Specifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.ShipmentManagement.Web.Pages;

public class RateShipmentModel : PageModel
{
	private readonly IMediator _mediator;
	private readonly IReadModel<Shipment> _readModel;

	public RateShipmentModel(IMediator mediator, IReadModel<Shipment> readModel)
	{
		_mediator = mediator;
		_readModel = readModel;
	}

	public Shipment Shipment { get; private set; } = default!;

	public SelectList CarriersList => new(Carrier.AllCarriers, "Code", "Name");

	[BindProperty]
	public RateRequest RateRequest { get; set; } = new();

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

		if (Shipment.CarrierRate is null)
		{
			RateRequest.Charges = Shipment.Details.Select(d => new ChargeRequest
			{
				Description = $"{d.HandlingUnitCount} {d.PackagingType.Name}, class {d.Class.Value}, {d.Weight} lbs."
			}).ToList();
		}
		else
		{
			RateRequest = new RateRequest
			{
				Charges = Shipment.CarrierRate.Charges.Select(chg => new ChargeRequest
				{
					Description = chg.Description,
					Amount = chg.Amount
				}).ToList(),
				Carrier = Shipment.CarrierRate.Carrier.Code,
				DiscountAmount = Shipment.CarrierRate.DiscountAmount,
				FuelCharge = Shipment.CarrierRate.FuelCharge
			};
		}

		return actionResult;
	}

	public async Task<IActionResult> OnPost()
	{
		if (ShipmentId == null)
		{
			return RedirectToPage("/errors/404", new { msg = "Shipment was not found." });
		}

		if (!ModelState.IsValid)
		{
			var (shipment, actionResult) = await QueryShipment(ShipmentId);
			if (shipment != null)
			{
				Shipment = shipment;
			}

			return actionResult;
		}

		_ = await _mediator.Send(new RateShipmentCommand(User, ShipmentId, RateRequest));

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