using EasyDdd.ShipmentManagement.Core;
using EasyDdd.ShipmentManagement.Core.CreateShipment;
using EasyDdd.ShipmentManagement.Web.Pages.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NodaTime;

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

	[BindProperty] public ShipmentRequest ShipmentRequest { get; init; } = CreateInitialFakeShipment();

	public void OnGet()
	{
	}

	public async Task<ActionResult> OnPost()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		// Because we have a static 5 detail lines and we only require 1 line to be filled out.
		ShipmentRequest.Details = ShipmentRequest.Details
			.Where(_ => _.Weight.HasValue && _.HandlingUnitCount.HasValue && _.Description != null)
			.ToList();

		if (!ShipmentRequest.Details.Any())
			ModelState.AddModelError(string.Empty, "At least 1 detail line is required to create a shipment.");

		var shipment = await _mediator.Send(new CreateShipmentCommand(User, ShipmentRequest));
		return RedirectToPage("/ShipmentSpotlight", new { id = shipment.Identifier });
	}

	private static ShipmentRequest CreateInitialFakeShipment() => new()
	{
		Shipper = new LocationRequest
		{
			Address = new AddressRequest
			{
				Line1 = "6450 Poe Ave.",
				Line2 = "Suite 414",
				City = "Dayton",
				StateAbbreviation = "OH",
				PostalCode = "45414"
			},
			Contact = new ContactRequest
			{
				Email = "rfoote@daytonfreight.com",
				Name = "Ryan Foote",
				Phone = "(800) 860-5102"
			}
		},
		Consignee = new LocationRequest
		{
			Address = new AddressRequest
			{
				Line1 = "7000 Kalahari Dr.",
				Line2 = "Ryans Room",
				City = "Sandusky",
				StateAbbreviation = "OH",
				PostalCode = "44870"
			},
			Contact = new ContactRequest
			{
				Email = "footedr@gmail.com",
				Name = "Ryan Foote",
				Phone = "(937) 604-1566"
			}
		},
		ReadyWindow = new AppointmentWindowRequest
		{
			Date = new LocalDate(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day),
			Start = "08:00",
			End = "17:00"
		},
		Details = new List<ShipmentDetailRequest>
		{
			new()
			{
				Class = "60",
				Description = "1 skid, class 60, 1000 lbs.",
				HandlingUnitCount = 1,
				IsHazardous = true,
				PackagingType = "SKID",
				Weight = 1000
			}
		}
	};
}