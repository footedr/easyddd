using System.Security.Claims;
using EasyDdd.Kernel;

namespace EasyDdd.Billing.Core.ApproveStatement
{
	public record ApproveStatementCommand(ClaimsPrincipal User, StatementIdentifier StatementIdentifier)
		: Command<Statement>;
}