using EasyDdd.Kernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Data
{
	public class TmsContext : DbContextWithDomainEvents
	{
		public TmsContext(DbContextOptions options, IMediator mediator) : base(options, mediator)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new ShipmentConfiguration());

			base.OnModelCreating(modelBuilder);
		}
	}
}