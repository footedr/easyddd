using System;
using EasyDdd.Core.CreateShipment;
using EasyDdd.Kernel;

namespace EasyDdd.Core
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

		public string Description { get; private set; }
		public FreightClass Class { get; private set; }
		public int Weight { get; private set; }
		public int HandlingUnitCount { get; private set; }
		public PackagingType PackagingType { get; private set; }
		public bool IsHazardous { get; private set; }

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