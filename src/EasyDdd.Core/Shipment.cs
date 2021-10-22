﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EasyDdd.Core.CreateShipment;
using EasyDdd.Core.Specifications;
using EasyDdd.Kernel;
using NodaTime;
using NodaTime.Text;

namespace EasyDdd.Core
{
	public class Shipment : Entity<string>
	{
		private readonly List<ShipmentDetail> _details = new();

		[Obsolete("Should only be used to rehydrate an entity")]
		private Shipment() : base(default!)
		{
			ReadyWindow = default!;
			Shipper = default!;
			Consignee = default!;
			Status = default!;
		}

		public Shipment(AppointmentWindowRequest readyWindow,
			LocationRequest shipper,
			LocationRequest consignee,
			IEnumerable<ShipmentDetailRequest> details,
			Instant createdAt) : base(Guid.NewGuid().ToString())
		{
			ReadyWindow = CreateAppointmentWindow(readyWindow);
			Shipper = new Location(CreateAddress(shipper.Address), CreateContact(shipper.Contact));
			Consignee = new Location(CreateAddress(consignee.Address), CreateContact(consignee.Contact));
			
			Status = ShipmentStatus.New;
			CreatedAt = createdAt;

			_details.AddRange(details.Select(CreateDetail));
			RecordEvent(new ShipmentCreated(this));
		}

		public AppointmentWindow ReadyWindow { get; }
		public Location Shipper { get; }
		public Location Consignee { get; }
		public IReadOnlyList<ShipmentDetail> Details => _details;
		public ShipmentStatus Status { get; }
		public Instant CreatedAt { get; }

		public ShipmentDetail AddDetail(ShipmentDetailRequest detail)
		{
			var shipmentDetail = CreateDetail(detail);
			
			_details.Add(shipmentDetail);
			RecordEvent(new ShipmentDetailAdded(Identifier, shipmentDetail));
			return shipmentDetail;
		}

		private static ShipmentDetail CreateDetail(ShipmentDetailRequest request)
		{
			var freightClass = FreightClass.Create(request.Class);
			var packagingType = PackagingType.Create(request.PackagingType);

			return new ShipmentDetail(freightClass, request.Weight, request.HandlingUnitCount, packagingType, request.IsHazardous, request.Description);
		}

		private static Contact CreateContact(ContactRequest request)
		{
			return new(request.Name)
			{
				Email = request.Email,
				Phone = request.Phone
			};
		}

		private static Address CreateAddress(AddressRequest request)
		{
			return new(request.Line1, request.City, request.StateAbbreviation, request.PostalCode)
			{
				Line2 = request.Line2
			};
		}

		private static AppointmentWindow CreateAppointmentWindow(AppointmentWindowRequest request)
		{
			var pattern = LocalTimePattern.Create("HH:ss", CultureInfo.InvariantCulture);
			var start = pattern.Parse(request.Start);
			var end = pattern.Parse(request.End);
			return new AppointmentWindow(request.Date, start.Value, end.Value);
		}
	}
}