using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Kernel
{
	public abstract class DbContextWithDomainEvents : DbContext
	{
		private readonly IMediator _mediator;

		protected DbContextWithDomainEvents(DbContextOptions options, IMediator mediator) : base(options)
		{
			_mediator = mediator;
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
				if (events.Count == 0) continue;

				foreach (var @event in events) await _mediator.Publish(@event, cancellationToken);
			}

			return count;
		}
	}
}