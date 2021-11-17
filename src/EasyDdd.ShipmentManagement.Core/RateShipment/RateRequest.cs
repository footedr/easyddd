using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyDdd.ShipmentManagement.Core.RateShipment
{
	public class RateRequest
	{
		[Required(ErrorMessage = "Carrier is required.")]
		public string? Carrier { get; set; }
		public List<ChargeRequest> Charges { get; set; } = new();
		[Required(ErrorMessage = "Fuel charge is required.")]
		public decimal? FuelCharge { get; set; }
		[Required(ErrorMessage = "Discount amount is required.")]
		public decimal? DiscountAmount { get; set; }
	}

	public class ChargeRequest
	{
		[Required(ErrorMessage = "Item charge amount is required.")]
		public decimal? Amount { get; set; }
		public string Description { get; set; } = default!;
	}
}