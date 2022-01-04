using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EasyDdd.ShipmentManagement.Core.CreateShipment;
using EasyDdd.ShipmentManagement.Core.DispatchShipment;
using EasyDdd.ShipmentManagement.Core.RateShipment;
using EasyDdd.ShipmentManagement.Core.Tracking;
using EasyDdd.Kernel;
using NodaTime;
using NodaTime.Text;

namespace EasyDdd.ShipmentManagement.Core;

public class Shipment : Entity<ShipmentId>
{
	private readonly List<ShipmentDetail> _details = new();
	private readonly List<TrackingEvent> _trackingHistory = new();

	[Obsolete("Should only be used to rehydrate an entity")]
	private Shipment() : base(default!)
	{
		ReadyWindow = default!;
		Shipper = default!;
		Consignee = default!;
		Status = default!;
		CreatedBy = default!;
	}

	public Shipment(ShipmentId id,
		AppointmentWindowRequest readyWindow,
		LocationRequest shipper,
		LocationRequest consignee,
		IEnumerable<ShipmentDetailRequest> details,
		Instant createdAt,
		string createdBy) : base(id)
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
	public Dispatch? DispatchInfo { get; private set; }
	public IReadOnlyList<TrackingEvent> TrackingHistory => _trackingHistory.OrderBy(evt => evt.Occurred).ToList();

	public ShipmentDetail AddDetail(ShipmentDetailRequest detail)
	{
		var shipmentDetail = CreateDetail(detail);

		_details.Add(shipmentDetail);
		RecordEvent(new ShipmentDetailAdded(Identifier, shipmentDetail));
		return shipmentDetail;
	}

	public ShipmentDetail UpdateLineItem(string detailIdentifier, ShipmentDetailRequest updatedDetail)
	{
		var detailToUpdate = _details.FirstOrDefault(item => item.Identifier.Equals(detailIdentifier, StringComparison.OrdinalIgnoreCase));
		if (detailToUpdate == null)
		{
			throw new NotFoundException<ShipmentDetail>(detailIdentifier);
		}

		ValidateShipmentDetail(updatedDetail);

		// Using dammit operator because ValidateShipmentDetail() will throw exception if desc, hu count, or weight is null.

		detailToUpdate.Class = updatedDetail.Class;
		detailToUpdate.Description = updatedDetail.Description!;
		detailToUpdate.HandlingUnitCount = updatedDetail.HandlingUnitCount!.Value;
		detailToUpdate.IsHazardous = updatedDetail.IsHazardous;
		detailToUpdate.PackagingType = updatedDetail.PackagingType;
		detailToUpdate.Weight = updatedDetail.Weight!.Value;

		RecordEvent(new ShipmentDetailUpdated(Identifier, detailToUpdate));

		return detailToUpdate;
	}

	public void Rate(RateRequest rateRequest)
	{
		if (rateRequest.Carrier == null)
		{
			throw new ArgumentNullException(nameof(rateRequest.Carrier), "Carrier is required.");
		}

		if (!rateRequest.Charges.Any() || rateRequest.Charges.Any(chg => !chg.Amount.HasValue))
		{
			throw new InvalidOperationException("Charges are required.");
		}

		if (rateRequest.Charges.Count != Details.Count)
		{
			throw new InvalidOperationException("A charge is required for each shipment detail line.");
		}

		if (rateRequest.DiscountAmount is null or <= 0)
		{
			throw new InvalidOperationException("Discount amount is required.");
		}

		if (rateRequest.FuelCharge is null or <= 0)
		{
			throw new InvalidOperationException("Fuel charge is required.");
		}

		CarrierRate = new Rate(rateRequest.Carrier, 
			rateRequest.FuelCharge.Value, 
			rateRequest.DiscountAmount.Value, 
			rateRequest.Charges.Select(chg => new Charge(chg.Amount!.Value, chg.Description)));

		RecordEvent(new ShipmentRated(Identifier, CarrierRate));

		UpdateStatus(ShipmentStatus.Rated);
	}

	public void Dispatch(DispatchNumber dispatchNumber, DispatchRequest dispatchRequest, string? createdBy, Instant created)
	{
		if (Status != ShipmentStatus.Rated)
		{
			throw new InvalidOperationException("Shipment must be in Rated status to be able to dispatch to carrier.");
		}

		if (dispatchRequest.PickupNumber is null)
		{
			throw new ArgumentNullException(nameof(dispatchRequest.PickupNumber), "Pickup number is required.");
		}

		if (string.IsNullOrWhiteSpace(createdBy))
		{
			throw new ArgumentNullException(nameof(createdBy), "Created by username is required.");
		}

		if (dispatchRequest.DispatchDate is null)
		{
			throw new ArgumentNullException(nameof(dispatchRequest.DispatchDate), "Dispatch date is required.");
		}

		if (dispatchRequest.DispatchTime is null)
		{
			throw new ArgumentNullException(nameof(dispatchRequest.DispatchDate), "Dispatch time is required.");
		}

		var dispatchDateTime = new LocalDateTime(dispatchRequest.DispatchDate.Value.Year,
			dispatchRequest.DispatchDate.Value.Month,
			dispatchRequest.DispatchDate.Value.Day,
			dispatchRequest.DispatchTime.Value.Hours,
			dispatchRequest.DispatchTime.Value.Minutes,
			dispatchRequest.DispatchTime.Value.Seconds);

		var dispatchInfo = new Dispatch(dispatchNumber,
			dispatchRequest.PickupNumber,
			dispatchDateTime,
			createdBy,
			created)
		{
			PickupNote = dispatchRequest.PickupNote,
			ReferenceNumber = dispatchRequest.ReferenceNumber
		};

		DispatchInfo = dispatchInfo;
		RecordEvent(new ShipmentDispatched(Identifier, DispatchInfo));

		UpdateStatus(ShipmentStatus.Dispatched);
	}

	public void AddTrackingEvent(TrackingEventRequest trackingEventRequest, string? createdBy, Instant occurred)
	{
		if (string.IsNullOrWhiteSpace(createdBy))
		{
			throw new ArgumentNullException(nameof(createdBy), "Created by username is required.");
		}

		if (string.IsNullOrWhiteSpace(trackingEventRequest.TypeCode))
		{
			throw new ArgumentNullException(nameof(trackingEventRequest.TypeCode), "Tracking event type code is required.");
		}

		if (trackingEventRequest.DeliveredDate is null)
		{
			throw new ArgumentNullException(nameof(trackingEventRequest.DeliveredDate), "Occurred date is required.");
		}

		if (trackingEventRequest.DeliveredTime is null)
		{
			throw new ArgumentNullException(nameof(trackingEventRequest.DeliveredTime), "Occurred time is required.");
		}
		
		var trackingEventType = TrackingEventType.Create(trackingEventRequest.TypeCode);
		var deliveredAt = new LocalDateTime(trackingEventRequest.DeliveredDate.Value.Year,
			trackingEventRequest.DeliveredDate.Value.Month,
			trackingEventRequest.DeliveredDate.Value.Day,
			trackingEventRequest.DeliveredTime.Value.Hours,
			trackingEventRequest.DeliveredTime.Value.Minutes,
			trackingEventRequest.DeliveredTime.Value.Seconds);

		var trackingEvent = new TrackingEvent(trackingEventType, deliveredAt, occurred, createdBy)
		{
			Comments = trackingEventRequest.Comments
		};

		if (_trackingHistory.Contains(trackingEvent))
		{
			return;
		}

		_trackingHistory.Add(trackingEvent);
		RecordEvent(new TrackingEventAdded(Identifier, trackingEvent));

		if (trackingEvent.Type.CorrespondingStatus is not null)
		{
			UpdateStatus(trackingEvent.Type.CorrespondingStatus);
		}

		if (Status == ShipmentStatus.Delivered)
		{
			RecordEvent(new ShipmentDelivered(Identifier, deliveredAt, occurred.ToDateTimeOffset()));
		}
	}

	private void UpdateStatus(ShipmentStatus status)
	{
		if (status == Status) return;

		var oldStatus = Status;
		Status = status;
		RecordEvent(new ShipmentStatusUpdated(Identifier, oldStatus, Status));
	}

	private static ShipmentDetail CreateDetail(ShipmentDetailRequest request)
	{
		ValidateShipmentDetail(request);

		var freightClass = FreightClass.Create(request.Class);
		var packagingType = PackagingType.Create(request.PackagingType);

		// Using dammit operator because ValidateShipmentDetail() will throw exception if desc, hu count, or weight is null.

		return new ShipmentDetail(freightClass, 
			request.Weight.Value, 
			request.HandlingUnitCount.Value, 
			packagingType, 
			request.IsHazardous, 
			request.Description);
	}

	private static void ValidateShipmentDetail(ShipmentDetailRequest request)
	{
		if (!request.Weight.HasValue
			|| !request.HandlingUnitCount.HasValue
			|| request.Description == null)
		{
			throw new InvalidOperationException("Weight, handling unit count, and description are required to create a shipment detail.");
		}
	}

	private static Contact CreateContact(ContactRequest request)
	{
		return new Contact(request.Name)
		{
			Email = request.Email,
			Phone = request.Phone
		};
	}

	private static Address CreateAddress(AddressRequest request)
	{
		return new Address(request.Line1, request.City, request.StateAbbreviation, request.PostalCode)
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