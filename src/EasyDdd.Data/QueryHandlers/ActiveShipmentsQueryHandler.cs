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

namespace EasyDdd.Data.QueryHandlers;

public record ActiveShipmentsQuery(ClaimsPrincipal User, Instant From, Instant To) : IRequest<IReadOnlyList<Shipment>>;

public class ActiveShipmentsQueryHandler : IRequestHandler<ActiveShipmentsQuery, IReadOnlyList<Shipment>>
{
	private readonly IReadModel<Shipment> _readModel;

	public ActiveShipmentsQueryHandler(IReadModel<Shipment> readModel)
	{
		_readModel = readModel;
	}

	public async Task<IReadOnlyList<Shipment>> Handle(ActiveShipmentsQuery query, CancellationToken cancellationToken)
	{
		return await _readModel.Query(query.User)
			.Where(new ActiveShipmentsSpecification(query.From, query.To).ToExpression())
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false);
	}
}