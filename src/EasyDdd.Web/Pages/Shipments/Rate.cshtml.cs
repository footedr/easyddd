using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyDdd.Core;
using EasyDdd.Core.RateShipment;
using EasyDdd.Core.Specifications;
using EasyDdd.Kernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Web.Pages.Shipments
{
	public class RateModel : PageModel
    {
		private readonly IMediator _mediator;
		private readonly IReadModel<Shipment> _readModel;

		public RateModel(IMediator mediator, IReadModel<Shipment> readModel)
		{
			_mediator = mediator;
			_readModel = readModel;
		}

		public Shipment Shipment { get; private set; } = default!;

		[BindProperty]
		public RateRequest RateRequest { get; set; } = new();

		[BindProperty]
		public string ShipmentIdentifier { get; set; } = default!;

		public async Task<IActionResult> OnGet(string id)
		{
			ShipmentIdentifier = id;
			var (shipment, actionResult) = await QueryShipment(ShipmentIdentifier);

			if (shipment != null)
			{
				Shipment = shipment;
			}
			
			return actionResult;
		}

		public async Task<IActionResult> OnPost()
		{
			if (!ModelState.IsValid)
			{
				var (shipment, actionResult) = await QueryShipment(ShipmentIdentifier);
				if (shipment != null)
				{
					Shipment = shipment;
				}

				return actionResult;
			}

			_ = await _mediator.Send(new RateShipmentCommand(User, ShipmentIdentifier, RateRequest));

			return RedirectToPage("/Shipments/Spotlight", new { id = ShipmentIdentifier });
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
}
