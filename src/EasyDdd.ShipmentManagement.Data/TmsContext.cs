using EasyDdd.Kernel;
using EasyDdd.Kernel.EventGrid;
using EasyDdd.Kernel.EventHubs;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.ShipmentManagement.Data
{
	public class TmsContext : DbContextWithDomainEvents
	{
		public TmsContext(DbContextOptions options, EventGridDomainEventProducer eventGridProducer, KafkaDomainEventProducer kafkaProducer) 
			: base(options, eventGridProducer, kafkaProducer)
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