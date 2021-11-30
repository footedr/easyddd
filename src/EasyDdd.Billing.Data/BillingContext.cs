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
			//modelBuilder.HasSequence("DispatchNumbers", "ShipmentManagement")
			//	.StartsAt(1000);

			//modelBuilder.ApplyConfiguration(new ShipmentConfiguration());

			base.OnModelCreating(modelBuilder);
		}
    }
}