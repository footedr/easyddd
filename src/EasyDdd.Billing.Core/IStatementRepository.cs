using EasyDdd.Kernel;

namespace EasyDdd.Billing.Core;

public interface IStatementRepository : IRepository<Statement>
{
	Task<StatementIdentifier> ReserveStatementIdentifier();
}