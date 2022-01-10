using System.Linq;
using System.Security.Claims;
using EasyDdd.ShipmentManagement.Core;
using EasyDdd.Kernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Billing.Data
{
	public class ShipmentsReadModel : IReadModel<Shipment>
	{
		private readonly BillingContext _billingContext;

		public ShipmentsReadModel(BillingContext billingContext)
		{
			_billingContext = billingContext;
		}

		public IQueryable<Shipment> Query(ClaimsPrincipal user)
		{
			return _billingContext.Set<Shipment>()
				.AsNoTracking();
		}
	}
}