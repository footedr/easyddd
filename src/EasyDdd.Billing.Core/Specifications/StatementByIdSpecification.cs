using System.Linq.Expressions;
using EasyDdd.Kernel;

namespace EasyDdd.Billing.Core.Specifications
{
	public class StatementByIdSpecification : Specification<Statement>
	{
		private readonly StatementIdentifier _id;

		public StatementByIdSpecification(StatementIdentifier id)
		{
			_id = id;
		}

		public override Expression<Func<Statement, bool>> ToExpression()
		{
			return statement => statement.Identifier == _id;
		}
	}
}