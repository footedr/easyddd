using System.Linq.Expressions;
using EasyDdd.Kernel;

namespace EasyDdd.Billing.Core.Specifications;

public class PendingStatementSpecification : Specification<Statement>
{
	private readonly string _billToAccount;
	private readonly string _billToLocation;
	private readonly string _customerCode;
	
	public PendingStatementSpecification(string customerCode, 
		string billToAccount, 
		string billToLocation)
	{
		_customerCode = customerCode;
		_billToAccount = billToAccount;
		_billToLocation = billToLocation;
	}

	public override Expression<Func<Statement, bool>> ToExpression()
	{
		return statement =>
			statement.CustomerCode == _customerCode &&
			statement.BillToAccount == _billToAccount &&
			statement.BillToLocation == _billToLocation &&
			statement.ProcessedAt == null;
	}
}