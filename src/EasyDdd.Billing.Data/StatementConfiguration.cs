using EasyDdd.Billing.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;

namespace EasyDdd.Billing.Data;

public class StatementConfiguration : BillingConfigurationBase<Statement>
{
	protected override string TableName => "Statements";

	protected override void ConfigureEntity(EntityTypeBuilder<Statement> builder)
	{
		builder.Property<int>("Id");
		builder.HasKey("Id");

		builder.Property(x => x.Version)
			.HasDefaultValueSql("1");
		builder.Property(x => x.Identifier)
			.HasConversion(
				id => id.Value,
				value => StatementIdentifier.Create(value));

		builder.Property(x => x.CustomerCode);

		builder.Property(x => x.BillToAccount);

		builder.Property(x => x.BillToLocation);

		builder.OwnsOne(x => x.BillingPeriod, billingPeriodBuilder =>
		{
			billingPeriodBuilder.Property(p => p.Start)
				.HasConversion(
					local => local.ToDateTimeUnspecified(),
					dateTime => LocalDate.FromDateTime(dateTime));

			billingPeriodBuilder.Property(p => p.End)
				.HasConversion(
					local => local.ToDateTimeUnspecified(),
					dateTime => LocalDate.FromDateTime(dateTime));
		});

		builder.Property(x => x.CreatedAt)
			.HasConversion(
				instant => instant.ToDateTimeOffset(),
				dateTime => Instant.FromDateTimeOffset(dateTime));

		builder.Property(x => x.ProcessedAt)
			.HasConversion(
				instant => instant.HasValue ? instant.Value.ToDateTimeOffset() : (DateTimeOffset?)null,
				dateTime => dateTime.HasValue ? Instant.FromDateTimeOffset(dateTime.Value) : null);

		builder.OwnsMany(x => x.Lines, lineBuilder =>
		{
			lineBuilder.ToTable("StatementLines", "billing");

			lineBuilder.Property<int>("Id");
			lineBuilder.HasKey("Id");

			lineBuilder.Property(l => l.TmsNumber).HasMaxLength(450);

			lineBuilder.Property(l => l.Description);

			lineBuilder.Property(l => l.Quantity);

			lineBuilder.Property(l => l.Amount)
				.HasColumnType("decimal(12,4)");

			lineBuilder.Property(l => l.Weight);

			lineBuilder.Property(l => l.Class)
				.HasMaxLength(450);

			lineBuilder.Property(l => l.TransactionDate)
				.HasConversion(
					local => local.ToDateTimeUnspecified(),
					dateTime => LocalDate.FromDateTime(dateTime));
		});

		builder.Navigation(x => x.Lines).AutoInclude(false);

		builder.HasIndex(x => x.Identifier);
		builder.HasIndex(x => x.CustomerCode);
		builder.HasIndex(x => x.BillToAccount);
		builder.HasIndex(x => x.BillToLocation);
		builder.HasIndex(x => x.ProcessedAt);
	}
}