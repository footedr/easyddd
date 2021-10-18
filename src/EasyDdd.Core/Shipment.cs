using System;
using System.Collections.Generic;
using System.Linq;
using EasyDdd.Kernel;

namespace EasyDdd.Core
{
	public class Shipment : Entity<string>
	{
		private readonly List<ShipmentDetailLine> _lineItems = new();

		[Obsolete("Should only be used to rehydrate an entity")]
		private Shipment() : base(default!)
		{
			ReadyWindow = default!;
			Shipper = default!;
			Consignee = default!;
		}

		public Shipment(AppointmentWindow readyWindow,
			Location shipper,
			Location consignee,
			IEnumerable<ShipmentDetailLineRequest> lineItems) : base(Guid.NewGuid().ToString())
		{
			ReadyWindow = readyWindow;
			Shipper = shipper;
			Consignee = consignee;
			_lineItems.AddRange(lineItems.Select(CreateLineItem));
			RecordEvent(new ShipmentCreated(this));
		}

		public AppointmentWindow ReadyWindow { get; }
		public Location Shipper { get; }
		public Location Consignee { get; }
		public IReadOnlyList<ShipmentDetailLine> LineItems => _lineItems;
		public bool IsHazardous => _lineItems.Any(_ => _.IsHazardous);
		public ShipmentStatus Status { get; private set; }

		public ShipmentDetailLine AddLineItem(ShipmentDetailLineRequest request)
		{
			var lineItem = CreateLineItem(request);

			_lineItems.Add(lineItem);

			RecordEvent(new LineItemAdded(Identifier, lineItem));

			return lineItem;
		}

		private ShipmentDetailLine CreateLineItem(ShipmentDetailLineRequest request)
		{
			return new(request.FreightClass, request.Weight, request.HandlingUnitCount, request.PackagingType, request.IsHazardous, request.Description);
		}
	}
}