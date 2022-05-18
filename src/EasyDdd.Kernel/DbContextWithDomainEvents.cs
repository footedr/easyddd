using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Kernel
{
	public abstract class DbContextWithDomainEvents : DbContext
	{
		private readonly IDomainEventProducer _domainEventProducer;
		
		protected DbContextWithDomainEvents(DbContextOptions options, IDomainEventProducer domainEventProducer) 
			: base(options)
		{
			_domainEventProducer = domainEventProducer;
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

				await _domainEventProducer.Produce(events, cancellationToken);
			}

			return count;
		}
	}
}