using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Billing.Data;

public class BillingContext : DbContext
{
	public BillingContext(DbContextOptions options)
		: base(options)
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