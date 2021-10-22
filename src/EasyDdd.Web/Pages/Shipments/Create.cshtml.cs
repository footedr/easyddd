using System.Threading.Tasks;
using EasyDdd.Core;
using EasyDdd.Core.CreateShipment;
using EasyDdd.Web.Pages.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace EasyDdd.Web.Pages.Shipments
{
	public class CreateModel : PageModel
	{
		private readonly ILogger<CreateModel> _logger;
		private readonly IMediator _mediator;
		public readonly SelectList FreightClassList;
		public readonly SelectList PackagingTypeList;

		public readonly SelectList StateList;

		public CreateModel(IMediator mediator, ILogger<CreateModel> logger)
		{
			_mediator = mediator;
			_logger = logger;
			StateList = new SelectList(States.All, "Name", "Abbreviation");
			FreightClassList = new SelectList(FreightClass.All, "Value", "Value");
			PackagingTypeList = new SelectList(PackagingType.All, "Code", "Name");
		}

		[BindProperty]
		public ShipmentRequest ShipmentRequest { get; set; } = new();

		public void OnGet()
		{
		}

		public async Task<ActionResult> OnPost()
		{
			if (ModelState.IsValid)
			{
				_ = await _mediator.Send(new CreateShipmentCommand(User, ShipmentRequest));
				return RedirectToPage("/Shipments/Index");
			}

			return Page();
		}
	}
}