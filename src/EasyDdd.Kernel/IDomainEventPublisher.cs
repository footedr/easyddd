namespace EasyDdd.Kernel
{
    public interface IDomainEventPublisher
    {
        Task PublishEvents(IReadOnlyList<DomainEvent> domainEvents, CancellationToken cancellationToken);
    }
}