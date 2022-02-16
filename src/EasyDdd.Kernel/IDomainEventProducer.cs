using System.Threading;
using System.Threading.Tasks;

namespace EasyDdd.Kernel;

public interface IDomainEventProducer
{
	Task Produce(DomainEvent domainEvent, CancellationToken cancellationToken);
}