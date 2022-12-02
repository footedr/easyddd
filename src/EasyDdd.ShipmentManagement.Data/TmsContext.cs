using EasyDdd.Kernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.ShipmentManagement.Data
{
	public class TmsContext : DbContextWithDomainEvents
	{
		public TmsContext(DbContextOptions options, IDomainEventPublisher domainEventPublisher) 
			: base(options, domainEventPublisher)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasSequence("DispatchNumbers", "ShipmentManagement")
				.StartsAt(1000);
			modelBuilder.HasSequence("ShipmentIds", "ShipmentManagement")
				.StartsAt(1000);
			modelBuilder.ApplyConfiguration(new ShipmentConfiguration());

			base.OnModelCreating(modelBuilder);
		}
	}
}