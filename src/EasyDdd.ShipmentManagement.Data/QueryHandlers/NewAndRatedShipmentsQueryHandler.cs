using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EasyDdd.ShipmentManagement.Core;
using EasyDdd.ShipmentManagement.Core.Specifications;
using EasyDdd.Kernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace EasyDdd.ShipmentManagement.Data.QueryHandlers
{
	public record NewAndRatedShipmentsQuery(ClaimsPrincipal User, Instant From, Instant To) : IRequest<IReadOnlyList<Shipment>>;

	public class NewAndRatedShipmentsQueryHandler : IRequestHandler<NewAndRatedShipmentsQuery, IReadOnlyList<Shipment>>
	{
		private readonly IReadModel<Shipment> _shipmentReadModel;

		public NewAndRatedShipmentsQueryHandler(IReadModel<Shipment> shipmentReadModel)
		{
			_shipmentReadModel = shipmentReadModel;
		}

		public async Task<IReadOnlyList<Shipment>> Handle(NewAndRatedShipmentsQuery request, CancellationToken cancellationToken)
		{
			var newShipments = await _shipmentReadModel.Query(request.User)
				.Where(new NewAndRatedShipmentsSpecification(request.From, request.To).ToExpression())
				.ToListAsync(cancellationToken);

			return newShipments;
		}
	}
}