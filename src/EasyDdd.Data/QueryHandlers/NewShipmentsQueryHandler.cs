using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EasyDdd.Core;
using EasyDdd.Core.Specifications;
using EasyDdd.Kernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace EasyDdd.Data.QueryHandlers
{
	public record NewShipmentsQuery(ClaimsPrincipal User, Instant From, Instant To) : IRequest<IReadOnlyList<Shipment>>;

	public class NewShipmentsQueryHandler : IRequestHandler<NewShipmentsQuery, IReadOnlyList<Shipment>>
	{
		private readonly IReadModel<Shipment> _shipmentReadModel;

		public NewShipmentsQueryHandler(IReadModel<Shipment> shipmentReadModel)
		{
			_shipmentReadModel = shipmentReadModel;
		}

		public async Task<IReadOnlyList<Shipment>> Handle(NewShipmentsQuery request, CancellationToken cancellationToken)
		{
			var newShipments = await _shipmentReadModel.Query(request.User)
				.Where(new NewShipmentsSpecification(request.From, request.To).ToExpression())
				.ToListAsync(cancellationToken);

			return newShipments;
		}
	}
}