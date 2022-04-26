using EasyDdd.Kernel;
using NodaTime;

namespace EasyDdd.Billing.Core;

public class Shipment : Entity<string>
{
	private List<ShipmentDetail> _details = new();

	[Obsolete("Should only be used to rehydrate an entity")]
	private Shipment() : base(default!)
	{
		Shipper = default!;
		Consignee = default!;
		CreatedBy = default!;
		Carrier = default!;
		TotalCost = default!;
		Status = default!;
	}

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
		CreatedBy = createdBy;
		Status = status;
		_details = details.GroupBy(_ => _.Class)
			.Select(grp => new ShipmentDetail(grp.Key, grp.Sum(_ => _.Weight), grp.Sum(_ => _.HandlingUnitCount), grp.Any(_ => _.IsHazardous)))
			.ToList();
	}

	public Address Shipper { get; }
	public Address Consignee { get; }
	public string CreatedBy { get; }
	public string Status { get; private set; }
	public decimal? TotalCost { get; private set; }
	public LocalDateTime? DeliveryDate { get; private set; }
	public Carrier? Carrier { get; private set; }
	public DispatchInfo? DispatchInfo { get; private set; }
	public TrackingEvent? LatesTrackingEvent { get; private set; }
	public IReadOnlyList<ShipmentDetail> Details => _details;

	public void UpdateRateInfo(Carrier carrier, decimal totalCharges)
	{
		TotalCost = totalCharges;
		Carrier = carrier;
	}

	public void UpdateDispatchInfo(DispatchInfo dispatchInfo)
	{
		DispatchInfo = dispatchInfo;
	}

	public void UpdateStatus(string status)
	{
		if (Status == status)
		{
			return;
		}

		Status = status;
	}

	public void UpdateDeliveryDate(LocalDateTime deliveryDate)
	{
		if (DeliveryDate == deliveryDate)
		{
			return;
		}

		DeliveryDate = deliveryDate;
	}

	/// <summary>
	///		Adds a shipment detail line item. Groups by freight class.
	/// </summary>
	/// <param name="detail"></param>
	public void AddDetail(ShipmentDetail detail)
	{
		// Find any existing line items for the given freight class.
		var detailMatchingClass = _details.SingleOrDefault(_ => _.Class == detail.Class);

		if (detailMatchingClass != null)
		{
			// Shipment already has a line item with the freight class of the item to add.
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

	public void UpdateDetails(IReadOnlyList<ShipmentDetail> details)
	{
		if (_details.Equals(details))
		{
			return;
		}

		_details = details.ToList();
	}

	public void UpdateLatestTrackingEvent(TrackingEvent trackingEvent)
	{
		if (LatesTrackingEvent == trackingEvent)
		{
			return;
		}

		LatesTrackingEvent = trackingEvent;
	}
}