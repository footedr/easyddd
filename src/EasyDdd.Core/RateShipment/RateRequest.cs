using System.Collections.Generic;

namespace EasyDdd.Core.RateShipment
{
	public class RateRequest
	{
		public List<ChargeRequest> Charges { get; set; } = new();
		public decimal? FuelCharge { get; set; }
		public decimal? DiscountAmount { get; set; }
	}
}