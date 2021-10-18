using System.Collections.Generic;

namespace EasyDdd.Kernel
{
	public interface IDomainEventSource
	{
		IReadOnlyList<DomainEvent> PublishEvents();
	}
}