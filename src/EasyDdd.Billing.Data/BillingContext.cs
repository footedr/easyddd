using EasyDdd.Kernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Billing.Data;

public class BillingContext : DbContextWithDomainEvents
{
	public BillingContext(DbContextOptions options, IDomainEventPublisher domainEventPublisher)
		: base(options, domainEventPublisher)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasSequence("StatementIdentifiers", "billing")
			.StartsAt(10000);

		modelBuilder.ApplyConfiguration(new ShipmentConfiguration());
		modelBuilder.ApplyConfiguration(new StatementConfiguration());

		base.OnModelCreating(modelBuilder);
	}
}