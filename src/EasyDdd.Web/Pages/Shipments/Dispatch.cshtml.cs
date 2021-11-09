using System.Linq;
using System.Threading.Tasks;
using EasyDdd.Core;
using EasyDdd.Core.DispatchShipment;
using EasyDdd.Core.Specifications;
using EasyDdd.Kernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Extensions;

namespace EasyDdd.Web.Pages.Shipments
{
    public class DispatchModel : PageModel
    {
		private readonly IMediator _mediator;
		private readonly IReadModel<Shipment> _readModel;
		private readonly IClock _clock;

		public DispatchModel(IMediator mediator, IReadModel<Shipment> readModel, IClock clock)
		{
			_mediator = mediator;
			_readModel = readModel;
			_clock = clock;
		}

		public Shipment Shipment { get; private set; } = default!;

		[BindProperty]
		public DispatchRequest? DispatchRequest { get; set; }

		[FromQuery]
		public string? ShipmentId { get; set; }

        public async Task<IActionResult> OnGet()
        {
			if (ShipmentId == null)
			{
				return RedirectToPage("/errors/404", new { msg = "Shipment was not found." });
			}

			var (shipment, actionResult) = await QueryShipment(ShipmentId);

			if (shipment == null) 
				return actionResult;

			Shipment = shipment;

			if (Shipment.Status == ShipmentStatus.Rated
				&& Shipment.CarrierRate != null)
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
			if (ShipmentId == null)
			{
				return RedirectToPage("/errors/404", new { msg = "Shipment was not found." });
			}

			var (shipment, actionResult) = await QueryShipment(ShipmentId);

			if (shipment == null) 
				return actionResult;

			Shipment = shipment;

			if (ModelState.IsValid && DispatchRequest is not null)
			{
				var dispatchShipmentCommand = new DispatchShipmentCommand(User, ShipmentId, DispatchRequest);
				_ = await _mediator.Send(dispatchShipmentCommand);
				return RedirectToPage("/Shipments/Spotlight", new { id = ShipmentId });
			}

			return actionResult;
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
}
