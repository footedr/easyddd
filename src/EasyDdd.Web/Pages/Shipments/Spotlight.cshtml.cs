using System.Threading.Tasks;
using EasyDdd.Core;
using EasyDdd.Core.Specifications;
using EasyDdd.Kernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Web.Pages.Shipments
{
	public class SpotlightModel : PageModel
	{
		private readonly IReadModel<Shipment> _readModel;

		public SpotlightModel(IReadModel<Shipment> readModel)
		{
			_readModel = readModel;
		}

		public Shipment Shipment { get; private set; } = default!;

		public async Task<IActionResult> OnGet(string id)
		{
			var shipment = await _readModel.Query(User)
				.SingleOrDefaultAsync(new ShipmentIdSpecification(id).ToExpression());

			if (shipment == null)
			{
				return RedirectToPage("/errors/404", new { msg = $"Shipment #{id} was not found." });
			}

			Shipment = shipment;

			return Page();
		}
	}
}