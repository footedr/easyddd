using System.Linq;
using System.Threading.Tasks;
using EasyDdd.ShipmentManagement.Core;
using EasyDdd.ShipmentManagement.Core.CreateShipment;
using EasyDdd.ShipmentManagement.Web.Pages.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EasyDdd.ShipmentManagement.Web.Pages;

public class CreateShipmentModel : PageModel
{
	private readonly IMediator _mediator;
	public readonly SelectList FreightClassList;
	public readonly SelectList PackagingTypeList;

	public readonly SelectList StateList;

	public CreateShipmentModel(IMediator mediator)
	{
		_mediator = mediator;
		StateList = new SelectList(States.All, "Name", "Abbreviation");
		FreightClassList = new SelectList(FreightClass.All, "Value", "Value");
		PackagingTypeList = new SelectList(PackagingType.All, "Code", "Name");
	}

	[BindProperty]
	public ShipmentRequest ShipmentRequest { get; init; } = new();

	public void OnGet()
	{
	}

	public async Task<ActionResult> OnPost()
	{
		if (!ModelState.IsValid) return Page();

		// Because we have a static 5 detail lines and we only require 1 line to be filled out.
		ShipmentRequest.Details = ShipmentRequest.Details
			.Where(_ => _.Weight.HasValue && _.HandlingUnitCount.HasValue && _.Description != null)
			.ToList();

		if (!ShipmentRequest.Details.Any()) ModelState.AddModelError(string.Empty, "At least 1 detail line is required to create a shipment.");

		var shipment = await _mediator.Send(new CreateShipmentCommand(User, ShipmentRequest));
		return RedirectToPage("/ShipmentSpotlight", new { id = shipment.Identifier });
	}
}