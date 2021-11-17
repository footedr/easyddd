using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyDdd.ShipmentManagement.Data.QueryHandlers;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NodaTime;

namespace EasyDdd.ShipmentManagement.Web.Pages;

public class IndexModel : PageModel
{
	private readonly IClock _clock;
	private readonly IMediator _mediator;
	public IReadOnlyList<ShipmentListItem> ActiveShipments = new List<ShipmentListItem>();
	public IReadOnlyList<ShipmentListItem> CompletedShipments = new List<ShipmentListItem>();

	public IReadOnlyList<ShipmentListItem> NewShipments = new List<ShipmentListItem>();

	public IndexModel(IMediator mediator, IClock clock)
	{
		_mediator = mediator;
		_clock = clock;
	}

	public async Task OnGet()
	{
		var start = _clock.GetCurrentInstant().Minus(Duration.FromDays(30));
		var end = _clock.GetCurrentInstant();

		NewShipments = (await _mediator.Send(new NewAndRatedShipmentsQuery(User, start, end)))
			.Select(_ => new ShipmentListItem(_.Identifier, _.Status.Description, _.ReadyWindow.ToDto(), _.Shipper.ToDto(), _.Consignee.ToDto()))
			.ToList();
		ActiveShipments = (await _mediator.Send(new ActiveShipmentsQuery(User, start, end)))
			.Select(_ => new ShipmentListItem(_.Identifier, _.Status.Description, _.ReadyWindow.ToDto(), _.Shipper.ToDto(), _.Consignee.ToDto()))
			.ToList();
		CompletedShipments = (await _mediator.Send(new CompletedShipmentsQuery(User, start, end)))
			.Select(_ => new ShipmentListItem(_.Identifier, _.Status.Description, _.ReadyWindow.ToDto(), _.Shipper.ToDto(), _.Consignee.ToDto()))
			.ToList();
	}
}