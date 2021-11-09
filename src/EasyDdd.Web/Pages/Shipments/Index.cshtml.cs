using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyDdd.Data.QueryHandlers;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace EasyDdd.Web.Pages.Shipments
{
	public class IndexModel : PageModel
	{
		private readonly IClock _clock;
		private readonly ILogger<IndexModel> _logger;
		private readonly IMediator _mediator;

		public IReadOnlyList<ShipmentListItem> NewShipments = new List<ShipmentListItem>();
		public IReadOnlyList<ShipmentListItem> ActiveShipments = new List<ShipmentListItem>();

		public IndexModel(IMediator mediator,
			IClock clock,
			ILogger<IndexModel> logger)
		{
			_mediator = mediator;
			_clock = clock;
			_logger = logger;
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
		}
	}
}