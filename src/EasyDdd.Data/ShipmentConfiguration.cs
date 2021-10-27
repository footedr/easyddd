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
				
			builder.Property(shipment => shipment.CreatedAt)
				.HasConversion(instant => instant.ToDateTimeUtc(),
					dateTime => Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)));

			builder.Property(shipment => shipment.CreatedBy)
				.IsRequired()
				.HasDefaultValue("SYSTEM");

			builder.HasIndex(shipment => shipment.Identifier).IsUnique();
			builder.HasIndex(shipment => shipment.Status);
		}
	}
}