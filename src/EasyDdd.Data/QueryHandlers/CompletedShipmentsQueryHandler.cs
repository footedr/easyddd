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

public record CompletedShipmentsQuery(ClaimsPrincipal User, Instant From, Instant To) : IRequest<IReadOnlyList<Shipment>>;

public class CompletedShipmentsQueryHandler : IRequestHandler<CompletedShipmentsQuery, IReadOnlyList<Shipment>>
{
	private readonly IReadModel<Shipment> _readModel;

	public CompletedShipmentsQueryHandler(IReadModel<Shipment> readModel)
	{
		_readModel = readModel;
	}

	public async Task<IReadOnlyList<Shipment>> Handle(CompletedShipmentsQuery query, CancellationToken cancellationToken)
	{
		return await _readModel.Query(query.User)
			.Where(new CompletedShipmentsSpecification(query.From, query.To).ToExpression())
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false);
	}
}