using System.Linq;
using System.Security.Claims;
using EasyDdd.ShipmentManagement.Core;
using EasyDdd.Kernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.ShipmentManagement.Data
{
	public class ShipmentsReadModel : IReadModel<Shipment>
	{
		private readonly TmsContext _tmsContext;

		public ShipmentsReadModel(TmsContext customersContext)
		{
			_tmsContext = customersContext;
		}

		public IQueryable<Shipment> Query(ClaimsPrincipal user)
		{
			return _tmsContext.Set<Shipment>()
				.AsNoTracking();
		}
	}
}