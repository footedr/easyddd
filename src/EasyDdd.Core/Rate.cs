using System.Collections.Generic;

namespace EasyDdd.Core
{
	public record Rate(IEnumerable<Charge> Charges)
	{
		public decimal? FuelCharge { get; init; }
		public decimal? DiscountAmount { get; init; }
	}

	public record Charge(decimal Amount)
	{
		public string? Description { get; init; }
	}
}