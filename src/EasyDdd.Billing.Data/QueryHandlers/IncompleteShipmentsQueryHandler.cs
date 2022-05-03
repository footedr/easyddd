using System.Security.Claims;
using EasyDdd.Billing.Core;
using EasyDdd.Billing.Core.Specifications;
using EasyDdd.Kernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Billing.Data.QueryHandlers;

public record IncompleteShipmentsQuery(ClaimsPrincipal User) : IRequest<IReadOnlyList<Shipment>>;

public class IncompleteShipmentsQueryHandler : IRequestHandler<IncompleteShipmentsQuery, IReadOnlyList<Shipment>>
{
	private readonly IReadModel<Shipment> _readModel;

	public IncompleteShipmentsQueryHandler(IReadModel<Shipment> readModel)
	{
		_readModel = readModel;
	}

	public async Task<IReadOnlyList<Shipment>> Handle(IncompleteShipmentsQuery request, CancellationToken cancellationToken)
	{
		var incompleteShipments = await _readModel.Query(request.User)
			.Where(new IncompleteShipmentsSpecification().ToExpression())
			.ToListAsync(cancellationToken);
		return incompleteShipments;
	}
}