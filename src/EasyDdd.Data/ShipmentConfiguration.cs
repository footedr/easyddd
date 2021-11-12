using System;
using EasyDdd.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;

namespace EasyDdd.Data
{
	public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
	{
		private const string Schema = "ShipmentManagement";
		
		public void Configure(EntityTypeBuilder<Shipment> builder)
		{
			builder.ToTable("Shipments", Schema);

			builder.Property<int>("Id");
			builder.HasKey("Id");

			builder.Property(shipment => shipment.Status)
				.HasConversion(status => status.Code,
					code => ShipmentStatus.Create(code));

			builder.OwnsOne(shipment => shipment.ReadyWindow)
				.Property(window => window.Date)
				.HasConversion(local => local.ToDateTimeUnspecified(),
					dateTime => LocalDate.FromDateTime(dateTime));
			builder.OwnsOne(shipment => shipment.ReadyWindow)
				.Property(window => window.Start)
				.HasConversion(time => new TimeSpan(0, time.Hour, time.Minute, time.Second, time.Millisecond),
					value => new LocalTime(value.Hours, value.Minutes, value.Seconds, value.Milliseconds));
			builder.OwnsOne(shipment => shipment.ReadyWindow)
				.Property(window => window.End)
				.HasConversion(time => new TimeSpan(0, time.Hour, time.Minute, time.Second, time.Millisecond),
					value => new LocalTime(value.Hours, value.Minutes, value.Seconds, value.Milliseconds));

			builder.OwnsOne(shipment => shipment.Shipper)
				.OwnsOne(location => location.Address);
			builder.OwnsOne(shipment => shipment.Shipper)
				.OwnsOne(location => location.Contact);

			builder.OwnsOne(shipment => shipment.Consignee)
				.OwnsOne(location => location.Address);
			builder.OwnsOne(shipment => shipment.Consignee)
				.OwnsOne(location => location.Contact);

			builder.OwnsMany(shipment => shipment.Details, detailBuilder =>
			{
				detailBuilder.ToTable("ShipmentDetails", Schema);

				detailBuilder.Property(detail => detail.Identifier);

				detailBuilder.Property(detail => detail.Class)
					.HasConversion(cls => cls.Value,
						str => FreightClass.Create(str));

				detailBuilder.Property(detail => detail.PackagingType)
					.HasConversion(pkgType => pkgType.Code,
						code => PackagingType.Create(code));

				detailBuilder.HasIndex(detail => detail.Identifier);
			});

			builder.OwnsOne(shipment => shipment.CarrierRate)
				.OwnsMany(rate => rate!.Charges, chargesBuilder =>
				{
					chargesBuilder.ToTable("RateItemCharges", Schema);

					chargesBuilder.Property(chg => chg.Amount)
						.HasColumnType("decimal(18,2)");
					chargesBuilder.Property(chg => chg.Description)
						.HasMaxLength(450);
				});
			builder.OwnsOne(shipment => shipment.CarrierRate)
				.Property(rate => rate!.Carrier)
				.HasColumnType("nvarchar(450)")
				.HasConversion(carrier => carrier.Code,
					code => Carrier.Create(code));
			builder.OwnsOne(shipment => shipment.CarrierRate)
				.Property(rate => rate!.DiscountAmount)
				.HasColumnType("decimal(18,2)");
			builder.OwnsOne(shipment => shipment.CarrierRate)
				.Property(rate => rate!.FuelCharge)
				.HasColumnType("decimal(18,2)");
			builder.OwnsOne(shipment => shipment.CarrierRate)
				.Property(rate => rate!.ChargeTotal)
				.HasColumnType("decimal(18,2)");
			builder.OwnsOne(shipment => shipment.CarrierRate)
				.Property(rate => rate!.DiscountAmount)
				.HasColumnType("decimal(18,2)");
			builder.OwnsOne(shipment => shipment.CarrierRate)
				.Property(rate => rate!.Total)
				.HasColumnType("decimal(18,2)");
				
			builder.Property(shipment => shipment.CreatedAt)
				.HasConversion(instant => instant.ToDateTimeUtc(),
					dateTime => Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)));

			builder.Property(shipment => shipment.CreatedBy)
				.IsRequired()
				.HasDefaultValue("SYSTEM");

			builder.OwnsOne(shipment => shipment.DispatchInfo)
				.Property(dsp => dsp!.DispatchNumber)
				.HasConversion(number => number.Value,
					value => DispatchNumber.Create(value));
			builder.OwnsOne(shipment => shipment.DispatchInfo)
				.Property(dsp => dsp!.DispatchDateTime)
				.HasConversion(localDate => localDate.ToDateTimeUnspecified(),
					dateTime => LocalDateTime.FromDateTime(dateTime));
			builder.OwnsOne(shipment => shipment.DispatchInfo)
				.Property(dsp => dsp!.Created)
				.HasConversion(instant => instant.ToDateTimeOffset(),
					dateTimeOffset => Instant.FromDateTimeOffset(dateTimeOffset))
				.HasDefaultValueSql("GETUTCDATE()");

			builder.OwnsMany(shipment => shipment.TrackingHistory, eventBuilder =>
			{
				eventBuilder.ToTable("TrackingHistory", Schema);

				eventBuilder.Property(evt => evt.Type)
					.HasConversion(type => type.Code, 
						code => TrackingEventType.Create(code));
				eventBuilder.Property(evt => evt.Occurred)
					.HasConversion(localDateTime => localDateTime.ToDateTimeUnspecified(),
						dateTime => LocalDateTime.FromDateTime(dateTime));
				eventBuilder.Property(evt => evt.CreatedAt)
					.HasConversion(instant => instant.ToDateTimeUtc(),
						dateTime => Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)));
				eventBuilder.Property(evt => evt.CreatedBy)
					.IsRequired();
				eventBuilder.Property(evt => evt.Comments)
					.IsRequired(false);

				eventBuilder.HasIndex(evt => evt.Type);
			});

			builder.HasIndex(shipment => shipment.Identifier).IsUnique();
			builder.HasIndex(shipment => shipment.Status);
		}
	}
}