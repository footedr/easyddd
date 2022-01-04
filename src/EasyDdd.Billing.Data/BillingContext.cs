using EasyDdd.Kernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Billing.Data
{
    public class BillingContext : DbContextWithDomainEvents
    {
		public BillingContext(DbContextOptions options, IMediator mediator) : base(options, mediator)
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
}