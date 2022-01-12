using System.Security.Claims;
using EasyDdd.Billing.Core;
using EasyDdd.Kernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Billing.Data
{
	public class StatementsReadModel : IReadModel<Statement>
	{
		private readonly BillingContext _billingContext;

		public StatementsReadModel(BillingContext billingContext)
		{
			_billingContext = billingContext;
		}

		public IQueryable<Statement> Query(ClaimsPrincipal user)
		{
			return _billingContext.Set<Statement>()
				.AsNoTracking();
		}
	}
}