using System.Threading.Tasks;
using EasyDdd.Kernel;
using EasyDdd.ShipmentManagement.Core;
using EasyDdd.ShipmentManagement.Core.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.ShipmentManagement.Web.Pages;

public class ShipmentSpotlightModel : PageModel
{
	private readonly IReadModel<Shipment> _readModel;

	public ShipmentSpotlightModel(IReadModel<Shipment> readModel)
	{
		_readModel = readModel;
	}

	[FromQuery]
	public string? Id { get; set; }

	public Shipment Shipment { get; private set; } = default!;

	public async Task<IActionResult> OnGet()
	{
		if (Id == null) return RedirectToPage("/errors/404", new { msg = "Shipment was not found." });

		var shipment = await _readModel.Query(User)
			.SingleOrDefaultAsync(new ShipmentIdSpecification(Id).ToExpression());

		if (shipment == null) return RedirectToPage("/errors/404", new { msg = $"Shipment #{Id} was not found." });

		Shipment = shipment;

		return Page();
	}
}