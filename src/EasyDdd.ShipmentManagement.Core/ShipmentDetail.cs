using System;
using EasyDdd.ShipmentManagement.Core.CreateShipment;
using EasyDdd.Kernel;

namespace EasyDdd.ShipmentManagement.Core
{
	/// <summary>
	///		Represents a detail line on a shipment, an actual shipping commodity.
	/// </summary>
	/// <remarks>
	///		Marked as an entity because you may want to track the detail as an entity. Like you may want to provide ability to edit a specific line. In this example though, it could easily just be a value object.
	/// </remarks>
	public class ShipmentDetail : Entity<string>
	{
		[Obsolete("Should only be used by EF")]
		private ShipmentDetail() : base(default!)
		{
			Description = default!;
			Class = default!;
			PackagingType = default!;
		}

		public ShipmentDetail(FreightClass freightClass, int weight, int handlingUnitCount, PackagingType packagingType, bool isHazardous, string description) 
			: base(Guid.NewGuid().ToString())
		{
			Class = freightClass;
			Weight = weight;
			HandlingUnitCount = handlingUnitCount;
			PackagingType = packagingType;
			IsHazardous = isHazardous;
			Description = description;
		}

		public string Description { get; set; }
		public FreightClass Class { get; set; }
		public int Weight { get; set; }
		public int HandlingUnitCount { get; set; }
		public PackagingType PackagingType { get; set; }
		public bool IsHazardous { get; set; }

		public ShipmentDetailRequest ToDto()
		{
			return new ShipmentDetailRequest
			{
				Class = Class,
				Description = Description,
				HandlingUnitCount = HandlingUnitCount,
				IsHazardous = IsHazardous,
				PackagingType = PackagingType,
				Weight = Weight
			};
		}
	}
}