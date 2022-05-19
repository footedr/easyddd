using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyDdd.Kernel;

public interface IDomainEventProducer
{
	Task Produce(IReadOnlyList<DomainEvent> domainEvents, CancellationToken cancellationToken);
}