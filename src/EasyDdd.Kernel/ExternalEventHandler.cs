using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace EasyDdd.Kernel;

public abstract class ExternalEventHandler<TEvent> : INotificationHandler<TEvent> where TEvent : ExternalEvent
{
	public abstract Task Handle(TEvent notification, CancellationToken cancellationToken);
}