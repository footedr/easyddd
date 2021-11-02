using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyDdd.Core;
using EasyDdd.Core.RateShipment;
using EasyDdd.Core.Specifications;
using EasyDdd.Kernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Web.Pages.Shipments
{
	public class RateModel : PageModel
    {
		private readonly IReadModel<Shipment> _readModel;

		public RateModel(IReadModel<Shipment> readModel)
		{
			_readModel = readModel;
		}

		public Shipment Shipment { get; private set; } = default!;

		[BindProperty]
		public RateRequest RateRequest { get; set; } = new();

		[FromQuery]
		public string? ShipmentId { get; set; }

		public async Task<IActionResult> OnGet()
		{
			if (ShipmentId is null)
			{
				throw new ArgumentNullException(nameof(ShipmentId), "Shipment id is missing.");
			}

			var (shipment, actionResult) = await QueryShipment(ShipmentId);

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
				return Page();
			}

			return Page();
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
