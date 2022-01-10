using System.Security.Claims;
using EasyDdd.Billing.Core;
using EasyDdd.Billing.Core.Specifications;
using EasyDdd.Kernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Billing.Data.QueryHandlers;

public record ProcessedStatementsQuery(ClaimsPrincipal User, string CustomerCode, string BillToAccount, string BillToLocation)
	: IRequest<IReadOnlyList<Statement>>;

public class ProcessedStatementsQueryHandler : IRequestHandler<ProcessedStatementsQuery, IReadOnlyList<Statement>>
{
	private readonly IReadModel<Statement> _readModel;

	public ProcessedStatementsQueryHandler(IReadModel<Statement> readModel)
	{
		_readModel = readModel;
	}

	public async Task<IReadOnlyList<Statement>> Handle(ProcessedStatementsQuery query, CancellationToken cancellationToken)
	{
		var statements = await _readModel.Query(query.User)
			.Where(new ProcessedStatementSpecification(query.CustomerCode, query.BillToAccount, query.BillToLocation).ToExpression())
			.ToListAsync(cancellationToken);
		return statements;
	}
}