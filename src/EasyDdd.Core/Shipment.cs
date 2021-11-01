using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EasyDdd.Core.CreateShipment;
using EasyDdd.Core.RateShipment;
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
			CreatedBy = default!;
		}

		public Shipment(AppointmentWindowRequest readyWindow,
			LocationRequest shipper,
			LocationRequest consignee,
			IEnumerable<ShipmentDetailRequest> details,
			Instant createdAt,
			string createdBy) : base(Guid.NewGuid().ToString())
		{
			ReadyWindow = CreateAppointmentWindow(readyWindow);
			Shipper = new Location(CreateAddress(shipper.Address), CreateContact(shipper.Contact));
			Consignee = new Location(CreateAddress(consignee.Address), CreateContact(consignee.Contact));
			
			Status = ShipmentStatus.New;
			CreatedAt = createdAt;
			CreatedBy = createdBy;

			_details.AddRange(details.Select(CreateDetail));

			if (!_details.Any())
			{
				throw new ArgumentException("At least 1 detail line is required to create a shipment.", nameof(details));
			}

			RecordEvent(new ShipmentCreated(this));
		}

		public AppointmentWindow ReadyWindow { get; }
		public Location Shipper { get; }
		public Location Consignee { get; }
		public IReadOnlyList<ShipmentDetail> Details => _details;
		public ShipmentStatus Status { get; private set; }
		public Instant CreatedAt { get; }
		public string CreatedBy { get; }
		public Rate? CarrierRate { get; private set; }

		public ShipmentDetail AddDetail(ShipmentDetailRequest detail)
		{
			var shipmentDetail = CreateDetail(detail);
			
			_details.Add(shipmentDetail);
			RecordEvent(new ShipmentDetailAdded(Identifier, shipmentDetail));
			return shipmentDetail;
		}

		public void Rate(RateRequest rateRequest)
		{
			if (!rateRequest.Charges.Any())
			{
				throw new InvalidOperationException("Charges are required.");
			}
			
			if (rateRequest.Charges.Count != Details.Count)
			{
				throw new InvalidOperationException("A charge is required for each shipment detail line.");
			}

			if (rateRequest.DiscountAmount <= 0)
			{
				throw new InvalidOperationException("Discount amount is required.");
			}

			if (rateRequest.FuelCharge <= 0)
			{
				throw new InvalidOperationException("Fuel charge is required.");
			}

			CarrierRate = new Rate(rateRequest.FuelCharge, rateRequest.DiscountAmount, rateRequest.Charges.Select(chg => new Charge(chg.Amount, chg.Description)));
			RecordEvent(new ShipmentRated(Identifier, CarrierRate));

			UpdateStatus(ShipmentStatus.Rated);
		}
		
		private void UpdateStatus(ShipmentStatus status)
		{
			if (status == Status)
			{
				return;
			}

			var oldStatus = Status;
			Status = status;
			RecordEvent(new ShipmentStatusUpdated(Identifier, oldStatus, Status));
		}

		private static ShipmentDetail CreateDetail(ShipmentDetailRequest request)
		{
			if (!request.Weight.HasValue || !request.HandlingUnitCount.HasValue || request.Description == null)
			{
				throw new InvalidOperationException("Weight, handling unit count, and description are required to create a shipment detail.");
			}

			var freightClass = FreightClass.Create(request.Class);
			var packagingType = PackagingType.Create(request.PackagingType);

			return new ShipmentDetail(freightClass, request.Weight.Value, request.HandlingUnitCount.Value, packagingType, request.IsHazardous, request.Description);
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