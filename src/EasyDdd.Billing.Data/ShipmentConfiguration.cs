using EasyDdd.Billing.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;

namespace EasyDdd.Billing.Data;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
	private const string Schema = "Billing";

	public void Configure(EntityTypeBuilder<Shipment> builder)
	{
		builder.ToTable("Shipments", Schema);

		builder.Property<int>("Id");
		builder.HasKey("Id");

		builder.Property(shipment => shipment.DeliveryDate)
			.HasConversion(local => local.HasValue ? local.Value.ToDateTimeUnspecified() : default,
				dateTime => LocalDateTime.FromDateTime(dateTime));

		builder.OwnsOne(shipment => shipment.Shipper);
		builder.OwnsOne(shipment => shipment.Consignee);
		builder.OwnsMany(shipment => shipment.Details, detailBuilder =>
		{
			detailBuilder.ToTable("ShipmentDetails", Schema);

			detailBuilder.Property<int>("Id");
			detailBuilder.HasKey("Id");
		});
		builder.OwnsOne(shipment => shipment.Carrier);
		builder.OwnsOne(shipment => shipment.DispatchInfo)
			.Property(dspInfo => dspInfo.DispatchDateTime)
			.HasConversion(local => local.ToDateTimeUnspecified(),
				dateTime => LocalDateTime.FromDateTime(dateTime));

		builder.HasIndex(shipment => shipment.Identifier).IsUnique();
		builder.HasIndex(shipment => shipment.Status);
	}
}