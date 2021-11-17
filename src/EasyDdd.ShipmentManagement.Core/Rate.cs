using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EasyDdd.Kernel;

namespace EasyDdd.ShipmentManagement.Core
{
	/// <summary>
	///		Represents a carrier rate quote on a shipment. Not marked as an entity because I'd like to treat this as a value object. If I get a new rate, I will just replace the old w/ the new vs. modifying.
	/// </summary>
	public class Rate : ValueObject
	{
		private readonly List<Charge> _charges = new();

		[Obsolete("Should only be used by EF")]
		private Rate()
		{
			Carrier = default!;
		}

		public Rate(Carrier carrier, decimal fuelCharge, decimal discountAmount, IEnumerable<Charge> charges)
		{
			Carrier = carrier;
			FuelCharge = fuelCharge;
			DiscountAmount = discountAmount;
			
			_charges.AddRange(charges);
			ChargeTotal = _charges.Sum(chg => chg.Amount);
			Total = ChargeTotal + FuelCharge - DiscountAmount;
		}

		public Carrier Carrier { get; }
		public decimal FuelCharge { get; private set; }
		public decimal DiscountAmount { get; private set; }
		public decimal ChargeTotal { get; }
		public decimal Total { get; }
		public IReadOnlyList<Charge> Charges => _charges;
		protected override ITuple AsTuple() => (FuelCharge, DiscountAmount, ChargeTotal, Total, Charges);
	}
	public record Charge(decimal Amount, string Description);
}