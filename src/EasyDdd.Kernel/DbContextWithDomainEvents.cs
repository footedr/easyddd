using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Kernel
{
	public abstract class DbContextWithDomainEvents : DbContext
	{
        private readonly IDomainEventPublisher _domainEventPublisher;

        protected DbContextWithDomainEvents(DbContextOptions options, IDomainEventPublisher domainEventPublisher) 
			: base(options)
		{
            _domainEventPublisher = domainEventPublisher;
		}

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var count = await base.SaveChangesAsync(cancellationToken);

            var domainEvents = ChangeTracker.Entries<IDomainEventSource>()
                .Select(e => e.Entity)
                .SelectMany(e => e.PublishEvents())
                .ToArray();

            await _domainEventPublisher.PublishEvents(domainEvents, cancellationToken);

            return count;
        }
	}
}