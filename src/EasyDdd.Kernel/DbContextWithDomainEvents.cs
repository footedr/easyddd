using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyDdd.Kernel.EventGrid;
using EasyDdd.Kernel.EventHubs;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Kernel
{
	public abstract class DbContextWithDomainEvents : DbContext
	{
		private readonly EventGridDomainEventProducer _eventGridProducer;
		private readonly KafkaDomainEventProducer _kafkaProducer;

		protected DbContextWithDomainEvents(DbContextOptions options, EventGridDomainEventProducer eventGridProducer, KafkaDomainEventProducer kafkaProducer) 
			: base(options)
		{
			_eventGridProducer = eventGridProducer;
			_kafkaProducer = kafkaProducer;
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var count = await base.SaveChangesAsync(cancellationToken);

			var entities = ChangeTracker.Entries<IDomainEventSource>()
				.Select(e => e.Entity)
				.ToArray();

			foreach (var entity in entities)
			{
				var events = entity.PublishEvents();
				
				if (events.Count == 0)
				{
					continue;
				}

				await Task.WhenAll(_kafkaProducer.Produce(events, cancellationToken), _eventGridProducer.Produce(events, cancellationToken));
			}

			return count;
		}
	}
}