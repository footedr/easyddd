using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace EasyDdd.Kernel
{
    public abstract class DomainEventHandler<TEvent> : INotificationHandler<TEvent> where TEvent : DomainEvent
    {
        public abstract Task Handle(TEvent notification, CancellationToken cancellationToken);
    }
}
