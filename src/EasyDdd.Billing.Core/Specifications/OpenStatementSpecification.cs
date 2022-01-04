using System.Linq.Expressions;
using EasyDdd.Kernel;
using NodaTime;

namespace EasyDdd.Billing.Core.Specifications;

public class OpenStatementSpecification : Specification<Statement>
{
	private readonly string _billToAccount;
	private readonly string _billToLocation;
	private readonly string _customerCode;
	private readonly LocalDate _transactionDate;

	public OpenStatementSpecification(string customerCode, string billToAccount, string billToLocation, LocalDate transactionDate)
	{
		_customerCode = customerCode;
		_billToAccount = billToAccount;
		_billToLocation = billToLocation;
		_transactionDate = transactionDate;
	}

	public override Expression<Func<Statement, bool>> ToExpression()
	{
		return statement =>
			statement.CustomerCode == _customerCode &&
			statement.BillToAccount == _billToAccount &&
			statement.BillToLocation == _billToLocation &&
			statement.ProcessedAt == null &&
			statement.BillingPeriod.End > _transactionDate;
	}
}