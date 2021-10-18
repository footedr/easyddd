using System;
using EasyDdd.Kernel;

namespace EasyDdd.Core
{
	public class ShipmentDetailLine : Entity<string>
	{
		public ShipmentDetailLine(FreightClass freightClass, int weight, int handlingUnitCount, PackagingType packagingType, bool isHazardous, string description) 
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
	}
}