using System.Linq;
using System.Threading.Tasks;
using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using EasyDdd.ShipmentManagement.Core.Specifications;
using EasyDdd.ShipmentManagement.Core.Tracking;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.ShipmentManagement.Web.Pages;

public class TrackingHistoryModel : PageModel
{
	private readonly IMediator _mediator;
	private readonly IReadModel<Shipment> _readModel;

	public TrackingHistoryModel(IReadModel<Shipment> readModel, IMediator mediator)
	{
		_readModel = readModel;
		_mediator = mediator;
	}

	[FromQuery]
	public string? ShipmentId { get; set; }

	[BindProperty]
	public TrackingEventRequest TrackingEventRequest { get; set; } = default!;

	public Shipment Shipment { get; private set; } = default!;

	public SelectList TrackingEventTypes => new(TrackingEventType.All, "Code", "Description");

	public async Task<IActionResult> OnGet()
	{
		if (ShipmentId == null) return RedirectToPage("/errors/404", new { msg = "Shipment was not found." });

		var shipment = await _readModel.Query(User)
			.SingleOrDefaultAsync(new ShipmentIdSpecification(ShipmentId).ToExpression());

		if (shipment == null) return RedirectToPage("/errors/404", new { msg = $"Shipment #{ShipmentId} was not found." });

		Shipment = shipment;

		return Page();
	}

	public async Task<IActionResult> OnPost()
	{
		if (ShipmentId == null) return RedirectToPage("/errors/404", new { msg = "Shipment was not found." });

		if (ModelState.IsValid) _ = await _mediator.Send(new AddTrackingEventCommand(User, ShipmentId, TrackingEventRequest));

		var (shipment, actionResult) = await QueryShipment(ShipmentId);

		if (shipment is null) return actionResult;

		Shipment = shipment;

		return Page();
	}

	private async Task<(Shipment? Shipment, IActionResult ActionResult)> QueryShipment(string shipmentIdentifier)
	{
		var shipment = await _readModel.Query(User)
			.Where(new ShipmentIdSpecification(shipmentIdentifier).ToExpression())
			.SingleOrDefaultAsync()
			.ConfigureAwait(false);

		if (shipment == null) return (default, RedirectToPage("/errors/404", new { msg = $"Shipment #{shipmentIdentifier} was not found." }));

		return (shipment, Page());
	}
}