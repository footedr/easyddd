using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyDdd.Kernel;

namespace EasyDdd.Billing.Core
{
    public class Shipment : Entity<string>
	{
		private readonly List<ShipmentDetail> _details;

		public Shipment(string identifier,
			Address shipper,
			Address consignee,
			string status,
			string createdBy,
			IReadOnlyList<ShipmentDetail> details) 
			: base(identifier)
		{
			Shipper = shipper;
			Consignee = consignee;
			Status = status;
			CreatedBy = createdBy;

			_details = details.GroupBy(_ => _.Class)
				.Select(grp => new ShipmentDetail(grp.Key, grp.Sum(_ => _.Weight), grp.Sum(_ => _.HandlingUnitCount), grp.Any(_ => _.IsHazardous)))
				.ToList();
		}

		public Address Shipper { get; }
		public Address Consignee { get; }
		public string Status { get; private set; }
		public string CreatedBy { get; }
		public IReadOnlyList<ShipmentDetail> Details => _details;

		public void AddDetail(ShipmentDetail detail)
		{
			var detailMatchingClass = _details.SingleOrDefault(_ => _.Class == detail.Class);
			if (detailMatchingClass != null)
			{
				var newDetail = new ShipmentDetail(detailMatchingClass.Class,
					detailMatchingClass.Weight + detail.Weight,
					detailMatchingClass.HandlingUnitCount + detail.HandlingUnitCount,
					detailMatchingClass.IsHazardous | detail.IsHazardous);
				var index = _details.IndexOf(detailMatchingClass);
				_details[index] = newDetail;
				return;
			}

			// Adding a detail w/ a freight class that is not currently on the shipment.
			_details.Add(detail);
		}
	}

	public record Address(string Line1, string City, string StateAbbreviation, string PostalCode);

	public record ShipmentDetail(string Class, int Weight, int HandlingUnitCount, bool IsHazardous);
}
